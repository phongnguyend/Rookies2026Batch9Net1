using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using ErrorOr;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;
using NashAssetManagement.Application.UseCases.Assets.Delete;
using NashAssetManagement.Application.UseCases.Assets.Specification;
using NashAssetManagement.Application.Abstractions.DataAccess;
using NashAssetManagement.Application.Abstractions.AppIdentity;
using NashAssetManagement.Application.Abstractions.DateTimes;
using NashAssetManagement.Domain.Entities.Core;
using NashAssetManagement.Domain.Enums;

public class DeleteAssetHandlerTests
{
    private readonly Mock<IRepository<Asset, Guid>> _assetRepoMock = new();
    private readonly Mock<IUnitOfWork> _uowMock = new();
    private readonly Mock<ICurrentUser> _currentUserMock = new();
    private readonly Mock<IDateTimeProvider> _dateTimeMock = new();
    private readonly Mock<ILogger<DeleteAssetHandler>> _loggerMock = new();

    private readonly DeleteAssetValidator _validator = new();

    private DeleteAssetHandler CreateHandler()
    {
        return new DeleteAssetHandler(
            _assetRepoMock.Object,
            _uowMock.Object,
            _validator,
            _currentUserMock.Object,
            _dateTimeMock.Object,
            _loggerMock.Object
        );
    }

    private static Asset CreateAsset(
        AssetState state = AssetState.Available,
        bool isDeleted = false,
        bool hasAssignments = false)
    {
        return new Asset
        {
            Id = Guid.NewGuid(),
            AssetCode = "LA000001",
            Name = "Laptop",
            LocationId = Guid.NewGuid(),
            State = state,
            IsDeleted = isDeleted,
            Assignments = hasAssignments
                ? new List<Assignment> { new Assignment() }
                : new List<Assignment>()
        };
    }

    [Fact]
    public async Task Handle_InvalidLocationId_ShouldReturnLocationNotFound()
    {
        _currentUserMock.Setup(x => x.LocationId).Returns("invalid-guid");

        var handler = CreateHandler();

        var result = await handler.Handle(
            new DeleteAssetRequest(Guid.NewGuid().ToString()),
            CancellationToken.None);

        Assert.True(result.IsError);
        Assert.Equal(DeleteAssetErrors.LocationNotFound, result.FirstError);
    }

    [Fact]
    public async Task Handle_AssetNotFound_ShouldReturnAssetNotFound()
    {
        _currentUserMock.Setup(x => x.LocationId).Returns(Guid.NewGuid().ToString());

        _assetRepoMock
            .Setup(x => x.FirstOrDefaultAsync(
                It.IsAny<DeleteAssetSpec>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync((Asset?)null);

        var handler = CreateHandler();

        var result = await handler.Handle(
            new DeleteAssetRequest(Guid.NewGuid().ToString()),
            CancellationToken.None);

        Assert.True(result.IsError);
        Assert.Equal(DeleteAssetErrors.AssetNotFound, result.FirstError);
    }

    [Fact]
    public async Task Handle_AssetIsAssigned_ShouldReturnAssetIsAssigned()
    {
        var locationId = Guid.NewGuid();

        _currentUserMock.Setup(x => x.LocationId).Returns(locationId.ToString());

        var asset = CreateAsset(state: AssetState.Assigned);

        _assetRepoMock
            .Setup(x => x.FirstOrDefaultAsync(
                It.IsAny<DeleteAssetSpec>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(asset);

        var handler = CreateHandler();

        var result = await handler.Handle(
            new DeleteAssetRequest(asset.Id.ToString()),
            CancellationToken.None);

        Assert.True(result.IsError);
        Assert.Equal(DeleteAssetErrors.AssetIsAssigned, result.FirstError);
    }

    [Fact]
    public async Task Handle_AssetHasHistory_ShouldReturnAssetHasHistory()
    {
        var locationId = Guid.NewGuid();

        _currentUserMock.Setup(x => x.LocationId).Returns(locationId.ToString());

        var asset = CreateAsset(hasAssignments: true);

        _assetRepoMock
            .Setup(x => x.FirstOrDefaultAsync(
                It.IsAny<DeleteAssetSpec>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(asset);

        var handler = CreateHandler();

        var result = await handler.Handle(
            new DeleteAssetRequest(asset.Id.ToString()),
            CancellationToken.None);

        Assert.True(result.IsError);
        Assert.Equal(DeleteAssetErrors.AssetHasHistory, result.FirstError);
    }

    [Fact]
    public async Task Handle_ValidRequest_ShouldDeleteAssetSuccessfully()
    {
        var locationId = Guid.NewGuid();

        _currentUserMock.Setup(x => x.LocationId).Returns(locationId.ToString());
        _dateTimeMock.Setup(x => x.UtcNow).Returns(DateTime.UtcNow);

        var asset = CreateAsset();

        _assetRepoMock
            .Setup(x => x.FirstOrDefaultAsync(
                It.IsAny<DeleteAssetSpec>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(asset);

        var handler = CreateHandler();

        var result = await handler.Handle(
            new DeleteAssetRequest(asset.Id.ToString()),
            CancellationToken.None);

        _assetRepoMock.Verify(x => x.UpdateDetached(It.IsAny<Asset>()), Times.Once);
        _uowMock.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);

        Assert.False(result.IsError);
        Assert.Equal(asset.Id, result.Value.Id);
    }
}