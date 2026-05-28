using FluentValidation;
using Microsoft.Extensions.Logging;
using Moq;
using NashAssetManagement.Application.Abstractions.AppIdentity;
using NashAssetManagement.Application.Abstractions.DataAccess;
using NashAssetManagement.Application.Abstractions.DateTimes;
using NashAssetManagement.Application.UseCases.Assets.Create;
using NashAssetManagement.Application.UseCases.Assets.Specification;
using NashAssetManagement.Application.UseCases.Categories.Specification;
using NashAssetManagement.Domain.Entities.Core;
using NashAssetManagement.Domain.Enums;
using Xunit;

namespace NashAssetManagement.UnitTests.Application.UseCases.Assets.Create;

public class HandlerTests
{
    private readonly Mock<IRepository<Asset, Guid>> _assetRepositoryMock = new();
    private readonly Mock<IRepository<Category, Guid>> _categoryRepositoryMock = new();
    private readonly Mock<IUnitOfWork> _unitOfWorkMock = new();
    private readonly Mock<ICurrentUser> _currentUserMock = new();
    private readonly Mock<IDateTimeProvider> _dateTimeProviderMock = new();
    private readonly Mock<ILogger<CreateAssetHandler>> _loggerMock = new();
    private readonly CreateAssetValidator _validator = new();
    private readonly CreateAssetHandler _handler;

    private readonly Guid _categoryId = Guid.NewGuid();
    private readonly Guid _locationId = Guid.NewGuid();

    public HandlerTests()
    {
        _currentUserMock
            .Setup(x => x.LocationId)
            .Returns(_locationId.ToString());

        _dateTimeProviderMock
            .Setup(x => x.UtcNow)
            .Returns(DateTime.UtcNow);

        _handler = new CreateAssetHandler(
            _assetRepositoryMock.Object,
            _categoryRepositoryMock.Object,
            _unitOfWorkMock.Object,
            _validator,
            _currentUserMock.Object,
            _dateTimeProviderMock.Object,
            _loggerMock.Object);
    }

    [Fact]
    public async Task CreateAssetHandler_RequestIsValid_ShouldCreateAsset()
    {
        // Arrange
        var request = CreateRequest();

        var category = new Category
        {
            Id = request.CategoryId,
            CategoryName = "Laptop",
            Prefix = "LA"
        };

        _categoryRepositoryMock
            .Setup(x => x.FirstOrDefaultAsync(
                It.IsAny<CategoryByIdSpec>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(category);

        _assetRepositoryMock
            .Setup(x => x.CountAsync(
                It.IsAny<AssetCountByCategorySpec>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(1);

        _assetRepositoryMock
            .Setup(x => x.AddAsync(
                It.IsAny<Asset>(),
                It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        _unitOfWorkMock
            .Setup(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(1);

        // Act
        var result = await _handler.Handle(request, CancellationToken.None);

        // Assert
        Assert.False(result.IsError);

        Assert.Equal("LA000002", result.Value.AssetCode);
        Assert.Equal("Laptop Dell", result.Value.AssetName);

        _assetRepositoryMock.Verify(
            x => x.AddAsync(
                It.IsAny<Asset>(),
                It.IsAny<CancellationToken>()),
            Times.Once);

        _unitOfWorkMock.Verify(
            x => x.SaveChangesAsync(It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task CreateAssetHandler_RequestIsInvalid_ShouldThrowValidationException()
    {
        // Arrange
        var request = CreateRequest() with
        {
            AssetName = ""
        };

        // Act & Assert
        await Assert.ThrowsAsync<ValidationException>(() =>
            _handler.Handle(request, CancellationToken.None));

        _categoryRepositoryMock.Verify(
            x => x.FirstOrDefaultAsync(
                It.IsAny<CategoryByIdSpec>(),
                It.IsAny<CancellationToken>()),
            Times.Never);

        _assetRepositoryMock.Verify(
            x => x.AddAsync(
                It.IsAny<Asset>(),
                It.IsAny<CancellationToken>()),
            Times.Never);
    }

    [Fact]
    public async Task CreateAssetHandler_LocationIdIsNull_ShouldReturnLocationNotFound()
    {
        // Arrange
        _currentUserMock
            .Setup(x => x.LocationId)
            .Returns((string?)null);

        var request = CreateRequest();

        // Act
        var result = await _handler.Handle(request, CancellationToken.None);

        // Assert
        Assert.True(result.IsError);

        Assert.Equal(
            CreateAssetErrors.LocationNotFound.Code,
            result.FirstError.Code);
    }

    [Fact]
    public async Task CreateAssetHandler_LocationIdIsInvalidGuid_ShouldReturnLocationNotFound()
    {
        // Arrange
        _currentUserMock
            .Setup(x => x.LocationId)
            .Returns("invalid-guid");

        var request = CreateRequest();

        // Act
        var result = await _handler.Handle(request, CancellationToken.None);

        // Assert
        Assert.True(result.IsError);

        Assert.Equal(
            CreateAssetErrors.LocationNotFound.Code,
            result.FirstError.Code);
    }

    [Fact]
    public async Task CreateAssetHandler_CategoryDoesNotExist_ShouldReturnCategoryNotFound()
    {
        // Arrange
        var request = CreateRequest();

        _categoryRepositoryMock
            .Setup(x => x.FirstOrDefaultAsync(
                It.IsAny<CategoryByIdSpec>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync((Category?)null);

        // Act
        var result = await _handler.Handle(request, CancellationToken.None);

        // Assert
        Assert.True(result.IsError);

        Assert.Equal(
            CreateAssetErrors.CategoryNotFound.Code,
            result.FirstError.Code);
    }

    [Fact]
    public async Task CreateAssetHandler_AssetCountExceedsLimit_ShouldReturnAssetCodeLimitReached()
    {
        // Arrange
        var request = CreateRequest();

        var category = new Category
        {
            Id = request.CategoryId,
            CategoryName = "Laptop",
            Prefix = "LA"
        };

        _categoryRepositoryMock
            .Setup(x => x.FirstOrDefaultAsync(
                It.IsAny<CategoryByIdSpec>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(category);

        _assetRepositoryMock
            .Setup(x => x.CountAsync(
                It.IsAny<AssetCountByCategorySpec>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(999999);

        // Act
        var result = await _handler.Handle(request, CancellationToken.None);

        // Assert
        Assert.True(result.IsError);

        Assert.Equal(
            CreateAssetErrors.AssetCodeLimitReached.Code,
            result.FirstError.Code);
    }

    [Fact]
    public async Task CreateAssetHandler_SaveChangesThrowsException_ShouldReturnAssetCreationFailed()
    {
        // Arrange
        var request = CreateRequest();

        var category = new Category
        {
            Id = request.CategoryId,
            CategoryName = "Laptop",
            Prefix = "LA"
        };

        _categoryRepositoryMock
            .Setup(x => x.FirstOrDefaultAsync(
                It.IsAny<CategoryByIdSpec>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(category);

        _assetRepositoryMock
            .Setup(x => x.CountAsync(
                It.IsAny<AssetCountByCategorySpec>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(1);

        _assetRepositoryMock
            .Setup(x => x.AddAsync(
                It.IsAny<Asset>(),
                It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        _unitOfWorkMock
            .Setup(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .ThrowsAsync(new Exception("Database error"));

        // Act
        var result = await _handler.Handle(request, CancellationToken.None);

        // Assert
        Assert.True(result.IsError);

        Assert.Equal(
            CreateAssetErrors.AssetCreationFailed.Code,
            result.FirstError.Code);
    }

    private CreateAssetRequest CreateRequest()
    {
        return new CreateAssetRequest(
            "Laptop Dell",
            "Core i7",
            DateTime.UtcNow.AddDays(-1),
            AssetState.Available,
            _categoryId);
    }
}