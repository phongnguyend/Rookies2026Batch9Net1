using FluentValidation;
using Microsoft.Extensions.Logging;
using Moq;
using NashAssetManagement.Application.Abstractions.AppIdentity;
using NashAssetManagement.Application.Abstractions.DataAccess;
using NashAssetManagement.Application.Abstractions.DateTimes;
using NashAssetManagement.Application.UseCases.Assets.Edit;
using NashAssetManagement.Application.UseCases.Assets.Specification;
using NashAssetManagement.Domain.Entities.Core;
using NashAssetManagement.Domain.Enums;
using Xunit;

namespace NashAssetManagement.UnitTests.Application.UseCases.Assets.Edit;

public class HandlerTests
{
    private readonly Mock<IRepository<Asset, Guid>> _assetRepository = new();
    private readonly Mock<IUnitOfWork> _unitOfWork = new();
    private readonly Mock<ICurrentUser> _currentUser = new();
    private readonly Mock<IDateTimeProvider> _dateTimeProvider = new();
    private readonly Mock<ILogger<EditAssetHandler>> _logger = new();

    private readonly EditAssetHandler _handler;

    public HandlerTests()
    {
        _handler = new EditAssetHandler(
            _assetRepository.Object,
            _unitOfWork.Object,
            new EditAssetValidator(),
            _currentUser.Object,
            _dateTimeProvider.Object,
            _logger.Object);
    }

    [Fact]
    public async Task Handle_InvalidLocationId_ShouldReturnLocationNotFound()
    {
        // Arrange
        _currentUser.Setup(x => x.LocationId)
            .Returns("invalid-guid");

        // Act
        var result = await _handler.Handle(
            CreateValidRequest(),
            CancellationToken.None);

        // Assert
        Assert.True(result.IsError);
        Assert.Contains(result.Errors,
            e => e.Code == EditAssetErrors.LocationNotFound.Code);
    }

    [Fact]
    public async Task Handle_InvalidAssetId_ShouldReturnAssetNotFound()
    {
        // Arrange
        _currentUser.Setup(x => x.LocationId)
            .Returns(Guid.NewGuid().ToString());

        var request = CreateValidRequest() with
        {
            AssetId = "invalid-guid"
        };

        // Act
        var result = await _handler.Handle(
            request,
            CancellationToken.None);

        // Assert
        Assert.True(result.IsError);
        Assert.Contains(result.Errors,
            e => e.Code == EditAssetErrors.AssetNotFound.Code);
    }

    [Fact]
    public async Task Handle_AssetNotFound_ShouldReturnAssetNotFound()
    {
        // Arrange
        _currentUser.Setup(x => x.LocationId)
            .Returns(Guid.NewGuid().ToString());

        _assetRepository
            .Setup(x => x.FirstOrDefaultAsync(
                It.IsAny<AssetByIdSpec>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync((Asset?)null);

        // Act
        var result = await _handler.Handle(
            CreateValidRequest(),
            CancellationToken.None);

        // Assert
        Assert.True(result.IsError);
        Assert.Contains(result.Errors,
            e => e.Code == EditAssetErrors.AssetNotFound.Code);
    }

    [Fact]
    public async Task Handle_AssignedAsset_ShouldReturnAssetNotEditable()
    {
        // Arrange
        _currentUser.Setup(x => x.LocationId)
            .Returns(Guid.NewGuid().ToString());

        var asset = CreateAsset();
        asset.State = AssetState.Assigned;

        _assetRepository
            .Setup(x => x.FirstOrDefaultAsync(
                It.IsAny<AssetByIdSpec>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(asset);

        // Act
        var result = await _handler.Handle(
            CreateValidRequest(),
            CancellationToken.None);

        // Assert
        Assert.True(result.IsError);
        Assert.Contains(result.Errors,
            e => e.Code == EditAssetErrors.AssetNotEditable.Code);
    }

    [Fact]
    public async Task Handle_InvalidRequest_ShouldThrowValidationException()
    {
        // Arrange
        _currentUser.Setup(x => x.LocationId)
            .Returns(Guid.NewGuid().ToString());

        var request = CreateValidRequest() with
        {
            AssetName = string.Empty
        };

        // Act & Assert
        await Assert.ThrowsAsync<ValidationException>(() =>
            _handler.Handle(request, CancellationToken.None));
    }

    [Fact]
    public async Task Handle_SaveChangesThrows_ShouldReturnAssetEditFailed()
    {
        // Arrange
        _currentUser.Setup(x => x.LocationId)
            .Returns(Guid.NewGuid().ToString());

        var asset = CreateAsset();

        _assetRepository
            .Setup(x => x.FirstOrDefaultAsync(
                It.IsAny<AssetByIdSpec>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(asset);

        _unitOfWork
            .Setup(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .ThrowsAsync(new Exception("Database error"));

        // Act
        var result = await _handler.Handle(
            CreateValidRequest(),
            CancellationToken.None);

        // Assert
        Assert.True(result.IsError);
        Assert.Contains(result.Errors,
            e => e.Code == EditAssetErrors.AssetEditFailed.Code);
    }

    [Fact]
    public async Task Handle_ValidRequest_ShouldUpdateAssetAndReturnResponse()
    {
        // Arrange
        var locationId = Guid.NewGuid();
        var updatedAt = new DateTime(2026, 1, 1);

        _currentUser.Setup(x => x.LocationId)
            .Returns(locationId.ToString());

        _dateTimeProvider.Setup(x => x.UtcNow)
            .Returns(updatedAt);

        var asset = CreateAsset();

        _assetRepository
            .Setup(x => x.FirstOrDefaultAsync(
                It.IsAny<AssetByIdSpec>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(asset);

        var request = CreateValidRequest();

        // Act
        var result = await _handler.Handle(
            request,
            CancellationToken.None);

        // Assert
        Assert.False(result.IsError);

        Assert.Equal(request.AssetName, asset.Name);
        Assert.Equal(request.Specification, asset.Specification);
        Assert.Equal(request.InstalledDate, asset.InstalledAtUtc);
        Assert.Equal(request.State, asset.State);
        Assert.Equal(updatedAt, asset.UpdatedAtUtc);

        _assetRepository.Verify(
            x => x.UpdateDetached(It.IsAny<Asset>()),
            Times.Once);

        _unitOfWork.Verify(
            x => x.SaveChangesAsync(It.IsAny<CancellationToken>()),
            Times.Once);

        Assert.Equal(asset.Id, result.Value.Id);
        Assert.Equal(asset.AssetCode, result.Value.AssetCode);
        Assert.Equal(asset.Name, result.Value.AssetName);
    }

    private static EditAssetRequest CreateValidRequest()
    {
        return new EditAssetRequest(
            AssetId: Guid.NewGuid().ToString(),
            AssetName: "Updated Laptop",
            Specification: "Updated Specification",
            InstalledDate: DateTime.UtcNow.AddDays(-1),
            State: AssetState.Available
        );
    }

    private static Asset CreateAsset()
    {
        return new Asset
        {
            Id = Guid.NewGuid(),
            AssetCode = "LA000001",
            Name = "Old Laptop",
            Specification = "Old Specification",
            InstalledAtUtc = DateTime.UtcNow.AddDays(-10),
            State = AssetState.Available,
            Category = new Category
            {
                CategoryName = "Laptop"
            },
            Location = new Location
            {
                Name = "HCM"
            }
        };
    }
}