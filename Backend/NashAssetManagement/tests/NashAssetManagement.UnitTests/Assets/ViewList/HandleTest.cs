using ErrorOr;
using FluentValidation;
using Moq;
using NashAssetManagement.Application.Abstractions.AppIdentity;
using NashAssetManagement.Application.Abstractions.DataAccess;
using NashAssetManagement.Application.UseCases.Assets.Specification;
using NashAssetManagement.Application.UseCases.Assets.ViewList;
using NashAssetManagement.Application.Utilities;
using NashAssetManagement.Domain.Entities.Core;
using NashAssetManagement.Domain.Enums;
using Xunit;
using static NashAssetManagement.Application.UseCases.Assets.Specification.AssetDetailSpec;

namespace NashAssetManagement.UnitTests.Application.UseCases.Assets.ViewList;

public class HandlerTests
{
    private readonly Mock<IRepository<Asset, Guid>> _assetRepositoryMock;
    private readonly Mock<IRepository<Category, Guid>> _categoryRepositoryMock;
    private readonly Mock<ICurrentUser> _currentUserMock;
    private readonly GetAssetsHandler _handler;
    private readonly GetAssetsValidator _validator;

    private readonly Guid _locationId = Guid.NewGuid();

    public HandlerTests()
    {
        _assetRepositoryMock = new Mock<IRepository<Asset, Guid>>();
        _categoryRepositoryMock = new Mock<IRepository<Category, Guid>>();
        _currentUserMock = new Mock<ICurrentUser>();

        _currentUserMock
            .Setup(u => u.LocationId)
            .Returns(_locationId.ToString());

        _categoryRepositoryMock
            .Setup(r => r.AnyAsync(
                It.IsAny<CategoryByNameSpec>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        _validator = new GetAssetsValidator(_categoryRepositoryMock.Object);

        _handler = new GetAssetsHandler(
            _assetRepositoryMock.Object,
            _validator,
            _currentUserMock.Object);
    }

    // ─── Happy path ───────────────────────────────────────
    [Fact]
    public async Task Handle_Should_Return_PagedList_When_Assets_Exist()
    {
        var request = new GetAssetsRequest(null, null, null, null, null);

        var assets = new List<GetAssetsResponse>
        {
            new(Guid.NewGuid(), "LA000001", "Laptop Dell XPS", "Laptop", AssetState.Available, "Ho Chi Minh"),
            new(Guid.NewGuid(), "MO000001", "Monitor LG 27", "Monitor", AssetState.Available, "Ho Chi Minh"),
        };

        _assetRepositoryMock
            .Setup(r => r.CountAsync(
                It.IsAny<AssetCountSpec>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(2);

        _assetRepositoryMock
            .Setup(r => r.ListAsync(
                It.IsAny<AssetSpec>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(assets);

        var result = await _handler.Handle(request, CancellationToken.None);

        Assert.False(result.IsError);
        Assert.Equal(2, result.Value.TotalCount);
        Assert.Equal(2, result.Value.Items.Count);
    }

    // ─── Empty result ─────────────────────────────────────
    [Fact]
    public async Task Handle_Should_Return_Empty_PagedList_When_No_Assets_Found()
    {
        var request = new GetAssetsRequest(
            Categories: null,
            States: null,
            Search: "nonexistent",   // ← named so position doesn't matter
            SortBy: null,
            SortDirection: null);

        _assetRepositoryMock
            .Setup(r => r.CountAsync(
                It.IsAny<AssetCountSpec>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(0);

        var result = await _handler.Handle(request, CancellationToken.None);

        Assert.False(result.IsError);
        Assert.Equal(0, result.Value.TotalCount);
        Assert.Empty(result.Value.Items);

        _assetRepositoryMock.Verify(
            r => r.ListAsync(
                It.IsAny<AssetSpec>(),
                It.IsAny<CancellationToken>()),
            Times.Never);
    }

    // ─── Validation fails ─────────────────────────────────
    [Fact]
    public async Task Handle_Should_Throw_ValidationException_When_PageNumber_Is_Invalid()
    {
        var request = new GetAssetsRequest(null, null, null, null, null, PageNumber: 0);

        await Assert.ThrowsAsync<ValidationException>(
            () => _handler.Handle(request, CancellationToken.None));
    }

    [Fact]
    public async Task Handle_Should_Throw_ValidationException_When_State_Is_Invalid()
    {
        var request = new GetAssetsRequest(null, ["InvalidState"], null, null, null);

        await Assert.ThrowsAsync<ValidationException>(
            () => _handler.Handle(request, CancellationToken.None));
    }

    [Fact]
    public async Task Handle_Should_Throw_ValidationException_When_SortBy_Is_Invalid()
    {
        var request = new GetAssetsRequest(null, null, null, "invalidField", null);

        await Assert.ThrowsAsync<ValidationException>(
            () => _handler.Handle(request, CancellationToken.None));
    }

    // ─── Repository not called when validation fails ───────
    [Fact]
    public async Task Handle_Should_Not_Call_Repository_When_Validation_Fails()
    {
        var request = new GetAssetsRequest(null, null, null, null, null, PageNumber: 0);

        await Assert.ThrowsAsync<ValidationException>(
            () => _handler.Handle(request, CancellationToken.None));

        _assetRepositoryMock.Verify(
            r => r.CountAsync(
                It.IsAny<AssetCountSpec>(),
                It.IsAny<CancellationToken>()),
            Times.Never);
    }

    // ─── Pagination ───────────────────────────────────────
    [Fact]
    public async Task Handle_Should_Return_Correct_Pagination_Metadata()
    {
        var request = new GetAssetsRequest(null, null, null, null, null, PageNumber: 2, PageSize: 5);

        _assetRepositoryMock
            .Setup(r => r.CountAsync(
                It.IsAny<AssetCountSpec>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(12);

        _assetRepositoryMock
            .Setup(r => r.ListAsync(
                It.IsAny<AssetSpec>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync([]);

        var result = await _handler.Handle(request, CancellationToken.None);

        Assert.False(result.IsError);
        Assert.Equal(12, result.Value.TotalCount);
        Assert.Equal(2, result.Value.PageNumber);
        Assert.Equal(5, result.Value.PageSize);
        Assert.Equal(3, result.Value.TotalPages);  // 12 / 5 = 3 pages
        Assert.True(result.Value.HasPreviousPage);
        Assert.True(result.Value.HasNextPage);
    }
}