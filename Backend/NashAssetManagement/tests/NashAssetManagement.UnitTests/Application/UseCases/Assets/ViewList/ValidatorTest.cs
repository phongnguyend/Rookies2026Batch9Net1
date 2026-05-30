using Moq;
using NashAssetManagement.Application.Abstractions.DataAccess;
using NashAssetManagement.Application.UseCases.Assets.Specification;
using NashAssetManagement.Application.UseCases.Assets.ViewList;
using NashAssetManagement.Application.UseCases.Categories.Specification;
using NashAssetManagement.Domain.Entities.Core;
using Xunit;

namespace NashAssetManagement.UnitTests.Application.UseCases.Assets.ViewList;

public class ValidatorTests
{
    private readonly Mock<IRepository<Category, Guid>> _categoryRepositoryMock;
    private readonly GetAssetsValidator _validator;

    public ValidatorTests()
    {
        _categoryRepositoryMock = new Mock<IRepository<Category, Guid>>();

        _categoryRepositoryMock
            .Setup(r => r.AnyAsync(
                It.IsAny<CategoryByNameSpec>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        _validator = new GetAssetsValidator(_categoryRepositoryMock.Object);
    }

    // ─── SortBy ───────────────────────────────────────────

    [Theory]
    [InlineData("assetcode")]
    [InlineData("name")]
    [InlineData("category")]
    [InlineData("state")]
    public async Task Validate_SortByIsValid_ShouldPass(string sortBy)
    {
        var request = new GetAssetsRequest(
            null,
            null,
            sortBy,
            null,
            null,
            false);

        var result = await _validator.ValidateAsync(request);

        Assert.True(result.IsValid);
    }

    [Fact]
    public async Task Validate_SortByIsInvalid_ShouldReturnError()
    {
        var request = new GetAssetsRequest(
            null,
            null,
            "invalidField",
            null,
            null,
            false);

        var result = await _validator.ValidateAsync(request);

        Assert.False(result.IsValid);

        Assert.Contains(result.Errors,
            x => x.PropertyName == "SortBy");
    }

    // ─── Search ───────────────────────────────────────────

    [Fact]
    public async Task Validate_SearchIsValid_ShouldPass()
    {
        var request = new GetAssetsRequest(
            null,
            null,
            null,
            null,
            "Laptop",
            false);

        var result = await _validator.ValidateAsync(request);

        Assert.True(result.IsValid);
    }

    [Fact]
    public async Task Validate_SearchIsNull_ShouldPass()
    {
        var request = new GetAssetsRequest(
            null,
            null,
            null,
            null,
            null,
            false);

        var result = await _validator.ValidateAsync(request);

        Assert.True(result.IsValid);
    }

    [Fact]
    public async Task Validate_SearchContainsOnlyWhitespace_ShouldReturnError()
    {
        var request = new GetAssetsRequest(
            null,
            null,
            null,
            null,
            "     ",
            false);

        var result = await _validator.ValidateAsync(request);

        Assert.False(result.IsValid);

        Assert.Contains(result.Errors,
            x => x.ErrorMessage == "Search cannot contain only whitespace.");
    }

    [Fact]
    public async Task Validate_SearchExceedsMaxLength_ShouldReturnError()
    {
        var search = new string('A', 101);

        var request = new GetAssetsRequest(
            null,
            null,
            null,
            null,
            search,
            false);

        var result = await _validator.ValidateAsync(request);

        Assert.False(result.IsValid);

        Assert.Contains(result.Errors,
            x => x.ErrorMessage == "Search must not exceed 100 characters.");
    }

    // ─── SortDirection ────────────────────────────────────

    [Theory]
    [InlineData("asc")]
    [InlineData("desc")]
    public async Task Validate_SortDirectionIsValid_ShouldPass(string sortDirection)
    {
        var request = new GetAssetsRequest(
            null,
            null,
            null,
            sortDirection,
            null,
            false);

        var result = await _validator.ValidateAsync(request);

        Assert.True(result.IsValid);
    }

    [Fact]
    public async Task Validate_SortDirectionIsInvalid_ShouldReturnError()
    {
        var request = new GetAssetsRequest(
            null,
            null,
            null,
            "invalidDirection",
            null,
            false);

        var result = await _validator.ValidateAsync(request);

        Assert.False(result.IsValid);

        Assert.Contains(result.Errors,
            x => x.PropertyName == "SortDirection");
    }

    // ─── States ───────────────────────────────────────────

    [Fact]
    public async Task Validate_StatesAreValid_ShouldPass()
    {
        var request = new GetAssetsRequest(
            null,
            ["Available", "Assigned"],
            null,
            null,
            null,
            false);

        var result = await _validator.ValidateAsync(request);

        Assert.True(result.IsValid);
    }

    [Fact]
    public async Task Validate_StatesAreInvalid_ShouldReturnError()
    {
        var request = new GetAssetsRequest(
            null,
            ["InvalidState"],
            null,
            null,
            null,
            false);

        var result = await _validator.ValidateAsync(request);

        Assert.False(result.IsValid);

        Assert.Contains(result.Errors,
            x => x.PropertyName == "States[0]");
    }

    // ─── Categories ───────────────────────────────────────

    [Fact]
    public async Task Validate_CategoryExists_ShouldPass()
    {
        var request = new GetAssetsRequest(
            ["Laptop"],
            null,
            null,
            null,
            null,
            false);

        var result = await _validator.ValidateAsync(request);

        Assert.True(result.IsValid);
    }

    [Fact]
    public async Task Validate_CategoryDoesNotExist_ShouldReturnError()
    {
        _categoryRepositoryMock
            .Setup(r => r.AnyAsync(
                It.IsAny<CategoryByNameSpec>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);

        var request = new GetAssetsRequest(
            ["Unicorn"],
            null,
            null,
            null,
            null,
            false);

        var result = await _validator.ValidateAsync(request);

        Assert.False(result.IsValid);

        Assert.Contains(result.Errors,
            x => x.ErrorMessage == "Category 'Unicorn' does not exist.");
    }

    // ─── Pagination ───────────────────────────────────────

    [Fact]
    public async Task Validate_PageNumberIsLessThanOne_ShouldReturnError()
    {
        var request = new GetAssetsRequest(
            null,
            null,
            null,
            null,
            null,
            false,
            PageNumber: 0);

        var result = await _validator.ValidateAsync(request);

        Assert.False(result.IsValid);

        Assert.Contains(result.Errors,
            x => x.ErrorMessage == "PageNumber must be at least 1.");
    }

    [Fact]
    public async Task Validate_PageSizeExceedsLimit_ShouldReturnError()
    {
        var request = new GetAssetsRequest(
            null,
            null,
            null,
            null,
            null,
            false,
            PageSize: 100);

        var result = await _validator.ValidateAsync(request);

        Assert.False(result.IsValid);

        Assert.Contains(result.Errors,
            x => x.ErrorMessage == "PageSize must be between 1 and 50.");
    }
}