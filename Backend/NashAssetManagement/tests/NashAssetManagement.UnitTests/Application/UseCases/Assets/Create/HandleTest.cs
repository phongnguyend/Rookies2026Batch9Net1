using FluentValidation;
using Moq;
using NashAssetManagement.Application.Abstractions.AppIdentity;
using NashAssetManagement.Application.Abstractions.DataAccess;
using NashAssetManagement.Application.UseCases.Assets.Create;
using NashAssetManagement.Application.UseCases.Assets.Specification;
using NashAssetManagement.Application.UseCases.Categories.Create;
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

    private readonly CreateAssetValidator _validator = new();

    private readonly CreateAssetHandler _handler;

    private readonly Guid _categoryId = Guid.NewGuid();
    private readonly Guid _locationId = Guid.NewGuid();

    public HandlerTests()
    {
        _currentUserMock
            .Setup(x => x.LocationId)
            .Returns(_locationId.ToString());

        _handler = new CreateAssetHandler(
            _assetRepositoryMock.Object,
            _categoryRepositoryMock.Object,
            _unitOfWorkMock.Object,
            _validator,
            _currentUserMock.Object);
    }

    // ─────────────────────────────
    // SUCCESS CASE
    // ─────────────────────────────
    // [Fact]
    // public async Task Handle_Should_CreateAsset_When_Request_IsValid()
    // {
    //     var request = CreateRequest();

    //     var category = new CreateCategoryResponse(
    //         _categoryId,
    //         "Laptop",
    //         "LA"
    //     );

    //     _categoryRepositoryMock
    //         .Setup(r => r.FirstOrDefaultAsync(
    //             It.IsAny<CategoryByIdSpec>(),
    //             It.IsAny<CancellationToken>()))
    //         .ReturnsAsync(category);

    //     _assetRepositoryMock
    //         .Setup(r => r.FirstOrDefaultAsync(
    //             It.IsAny<AssetCountByCategorySpec>(),
    //             It.IsAny<CancellationToken>()))
    //         .ReturnsAsync("LA000001");

    //     _assetRepositoryMock
    //         .Setup(r => r.AddAsync(
    //             It.IsAny<Asset>(),
    //             It.IsAny<CancellationToken>()))
    //         .Returns(Task.CompletedTask);

    //     _unitOfWorkMock
    //         .Setup(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()))
    //         .ReturnsAsync(1);

    //     var result = await _handler.Handle(request, CancellationToken.None);

    //     Assert.False(result.IsError);
    //     Assert.Equal("LA000002", result.Value.AssetCode);
    // }

    // ─────────────────────────────
    // VALIDATION FAILURE
    // ─────────────────────────────
    [Fact]
    public async Task Handle_Should_ThrowValidationException_When_Request_IsInvalid()
    {
        var request = CreateRequest() with
        {
            AssetName = ""
        };

        await Assert.ThrowsAsync<ValidationException>(() =>
            _handler.Handle(request, CancellationToken.None));

        _categoryRepositoryMock.Verify(
            r => r.FirstOrDefaultAsync(
                It.IsAny<CategoryByIdSpec>(),
                It.IsAny<CancellationToken>()),
            Times.Never);

        _assetRepositoryMock.Verify(
            r => r.AddAsync(
                It.IsAny<Asset>(),
                It.IsAny<CancellationToken>()),
            Times.Never);
    }

    // ─────────────────────────────
    // LOCATION NULL
    // ─────────────────────────────
    [Fact]
    public async Task Handle_Should_ReturnLocationNotFound_When_LocationMissing()
    {
        _currentUserMock.Setup(x => x.LocationId).Returns((string?)null);

        var request = CreateRequest();

        var result = await _handler.Handle(request, CancellationToken.None);

        Assert.True(result.IsError);
        Assert.Equal(CreateAssetErrors.LocationNotFound.Code, result.FirstError.Code);
    }

    // ─────────────────────────────
    // CATEGORY NOT FOUND
    // ─────────────────────────────
    [Fact]
    public async Task Handle_Should_ReturnCategoryNotFound_When_CategoryMissing()
    {
        var request = CreateRequest();

        _categoryRepositoryMock
            .Setup(r => r.FirstOrDefaultAsync(
                It.IsAny<CategoryByIdSpec>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync((CreateCategoryResponse?)null);

        var result = await _handler.Handle(request, CancellationToken.None);

        Assert.True(result.IsError);
        Assert.Equal(CreateAssetErrors.CategoryNotFound.Code, result.FirstError.Code);
    }

    // ─────────────────────────────
    // INVALID CODE FORMAT
    // ─────────────────────────────
    [Fact]
    public async Task Handle_Should_ReturnInvalidCodeFormat_When_CodeInvalid()
    {
        var request = CreateRequest();

        var category = new CreateCategoryResponse(
            _categoryId,
            "Laptop",
            "LA"
        );

        _categoryRepositoryMock
            .Setup(r => r.FirstOrDefaultAsync(
                It.IsAny<CategoryByIdSpec>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(category);

        // _assetRepositoryMock
        //     .Setup(r => r.FirstOrDefaultAsync(
        //         It.IsAny<AssetCountByCategorySpec>(),
        //         It.IsAny<CancellationToken>()))
        //     .ReturnsAsync("LAABC001");

        var result = await _handler.Handle(request, CancellationToken.None);

        Assert.True(result.IsError);
        Assert.Equal(
            CreateAssetErrors.InvalidAssetCodeFormat.Code,
            result.FirstError.Code);
    }

    // ─────────────────────────────
    // HELPER
    // ─────────────────────────────
    private CreateAssetRequest CreateRequest()
    {
        return new CreateAssetRequest(
            "Laptop Dell",
            "Core i7",
            DateTime.UtcNow.AddDays(-1),
            AssetState.Available,
            _categoryId
        );
    }
}