using FluentValidation;
using Moq;
using NashAssetManagement.Application.Abstractions.AppIdentity;
using NashAssetManagement.Application.Abstractions.DataAccess;
using NashAssetManagement.Application.UseCases.Assets.Specification;
using NashAssetManagement.Application.UseCases.Assets.ViewList;
using NashAssetManagement.Application.UseCases.Categories.Specification;
using NashAssetManagement.Domain.Entities.Core;
using NashAssetManagement.Domain.Enums;
using Xunit;

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
            .Setup(x => x.LocationId)
            .Returns(_locationId.ToString());

        _categoryRepositoryMock
            .Setup(x => x.AnyAsync(
                It.IsAny<CategoryByNameSpec>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        _validator = new GetAssetsValidator(_categoryRepositoryMock.Object);

        _handler = new GetAssetsHandler(
            _assetRepositoryMock.Object,
            _validator,
            _currentUserMock.Object);
    }

    // ─── Happy Path ───────────────────────────────────────

    [Fact]
    public async Task Handle_AssetsExist_ShouldReturnPagedList()
    {
        var request = new GetAssetsRequest(
            Categories: null,
            States: null,
            SortBy: null,
            SortDirection: null,
            Search: null);

        var assets = new List<GetAssetsResponse>
        {
            new(
                Guid.NewGuid(),
                "LA000001",
                "Laptop Dell XPS",
                "Laptop",
                AssetState.Available,
                "Ho Chi Minh"),

            new(
                Guid.NewGuid(),
                "MO000001",
                "Monitor LG 27",
                "Monitor",
                AssetState.Available,
                "Ho Chi Minh"),
        };

        _assetRepositoryMock
            .Setup(x => x.CountAsync(
                It.IsAny<AssetCountSpec>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(2);

        _assetRepositoryMock
            .Setup(x => x.ListAsync(
                It.IsAny<AssetListSpec>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(assets);

        var result = await _handler.Handle(request, CancellationToken.None);

        Assert.False(result.IsError);

        Assert.Equal(2, result.Value.TotalCount);
        Assert.Equal(2, result.Value.Items.Count);
    }

    // ─── Empty Result ────────────────────────────────────

    [Fact]
    public async Task Handle_NoAssetsFound_ShouldReturnEmptyPagedList()
    {
        var request = new GetAssetsRequest(
            Categories: null,
            States: null,
            SortBy: null,
            SortDirection: null,
            Search: "nonexistent");

        _assetRepositoryMock
            .Setup(x => x.CountAsync(
                It.IsAny<AssetCountSpec>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(0);

        var result = await _handler.Handle(request, CancellationToken.None);

        Assert.False(result.IsError);

        Assert.Equal(0, result.Value.TotalCount);
        Assert.Empty(result.Value.Items);

        _assetRepositoryMock.Verify(
            x => x.ListAsync(
                It.IsAny<AssetListSpec>(),
                It.IsAny<CancellationToken>()),
            Times.Never);
    }

    // ─── Validation ──────────────────────────────────────

    [Fact]
    public async Task Handle_PageNumberInvalid_ShouldThrowValidationException()
    {
        var request = new GetAssetsRequest(
            Categories: null,
            States: null,
            SortBy: null,
            SortDirection: null,
            Search: null,
            PageNumber: 0);

        await Assert.ThrowsAsync<ValidationException>(
            () => _handler.Handle(request, CancellationToken.None));
    }

    [Fact]
    public async Task Handle_StateInvalid_ShouldThrowValidationException()
    {
        var request = new GetAssetsRequest(
            Categories: null,
            States: ["InvalidState"],
            SortBy: null,
            SortDirection: null,
            Search: null);

        await Assert.ThrowsAsync<ValidationException>(
            () => _handler.Handle(request, CancellationToken.None));
    }

    [Fact]
    public async Task Handle_SortByInvalid_ShouldThrowValidationException()
    {
        var request = new GetAssetsRequest(
            Categories: null,
            States: null,
            SortBy: "invalidField",
            SortDirection: null,
            Search: null);

        await Assert.ThrowsAsync<ValidationException>(
            () => _handler.Handle(request, CancellationToken.None));
    }

    [Fact]
    public async Task Handle_SortDirectionInvalid_ShouldThrowValidationException()
    {
        var request = new GetAssetsRequest(
            Categories: null,
            States: null,
            SortBy: null,
            SortDirection: "invalidDirection",
            Search: null);

        await Assert.ThrowsAsync<ValidationException>(
            () => _handler.Handle(request, CancellationToken.None));
    }

    [Fact]
    public async Task Handle_SearchContainsOnlyWhitespace_ShouldThrowValidationException()
    {
        var request = new GetAssetsRequest(
            Categories: null,
            States: null,
            SortBy: null,
            SortDirection: null,
            Search: "     ");

        await Assert.ThrowsAsync<ValidationException>(
            () => _handler.Handle(request, CancellationToken.None));
    }

    [Fact]
    public async Task Handle_SearchExceedsMaxLength_ShouldThrowValidationException()
    {
        var request = new GetAssetsRequest(
            Categories: null,
            States: null,
            SortBy: null,
            SortDirection: null,
            Search: new string('A', 101));

        await Assert.ThrowsAsync<ValidationException>(
            () => _handler.Handle(request, CancellationToken.None));
    }

    // ─── Repository Call Protection ──────────────────────

    [Fact]
    public async Task Handle_ValidationFails_ShouldNotCallRepository()
    {
        var request = new GetAssetsRequest(
            Categories: null,
            States: null,
            SortBy: null,
            SortDirection: null,
            Search: null,
            PageNumber: 0);

        await Assert.ThrowsAsync<ValidationException>(
            () => _handler.Handle(request, CancellationToken.None));

        _assetRepositoryMock.Verify(
            x => x.CountAsync(
                It.IsAny<AssetCountSpec>(),
                It.IsAny<CancellationToken>()),
            Times.Never);
    }

    // ─── Pagination ──────────────────────────────────────

    [Fact]
    public async Task  Handle_Pagination_ShouldReturnCorrectMetadata()
    {
        var request = new GetAssetsRequest(
            Categories: null,
            States: null,
            SortBy: null,
            SortDirection: null,
            Search: null,
            PageNumber: 2,
            PageSize: 5);

        _assetRepositoryMock
            .Setup(x => x.CountAsync(
                It.IsAny<AssetCountSpec>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(12);

        _assetRepositoryMock
            .Setup(x => x.ListAsync(
                It.IsAny<AssetListSpec>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync([]);

        var result = await _handler.Handle(request, CancellationToken.None);

        Assert.False(result.IsError);

        Assert.Equal(12, result.Value.TotalCount);
        Assert.Equal(2, result.Value.PageNumber);
        Assert.Equal(5, result.Value.PageSize);
        Assert.Equal(3, result.Value.TotalPages);

        Assert.True(result.Value.HasPreviousPage);
        Assert.True(result.Value.HasNextPage);
    }
}