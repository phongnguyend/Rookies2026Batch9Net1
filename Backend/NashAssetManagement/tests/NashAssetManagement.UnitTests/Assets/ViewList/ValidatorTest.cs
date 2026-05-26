using FluentValidation;
using Moq;
using NashAssetManagement.Application.Abstractions.DataAccess;
using NashAssetManagement.Application.UseCases.Assets.Specification;
using NashAssetManagement.Application.UseCases.Assets.ViewList;
using NashAssetManagement.Domain.Entities.Core;
using Xunit;
using static NashAssetManagement.Application.UseCases.Assets.Specification.AssetDetailSpec;

namespace NashAssetManagement.UnitTests.Application.UseCases.Assets.ViewList;

public class ValidatorTests
{
    private readonly Mock<IRepository<Category, Guid>> _categoryRepositoryMock;
    private readonly GetAssetsValidator _validator;
    private static readonly string[] AllowedSortBy = ["assetcode", "name", "category", "state"];
    private static readonly string[] AllowedSortDirection = ["asc", "desc"];

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
    public async Task Validate_Should_Pass_When_SortBy_Is_Valid(string sortBy)
    {
        var request = new GetAssetsRequest(null, null, null, sortBy, null);

        var result = await _validator.ValidateAsync(request);

        Assert.True(result.IsValid);
    }

    [Fact]
    public async Task Validate_Should_Return_Error_When_SortBy_Is_Invalid()
    {
        var request = new GetAssetsRequest(null, null, null, "invalidField", null);

        var result = await _validator.ValidateAsync(request);

        Assert.False(result.IsValid);
        Assert.Contains(result.Errors,
            x => x.ErrorMessage.Contains("SortBy must be one of"));  // ← contains instead of exact match
    }

    [Fact]
    public async Task Validate_Should_Pass_When_SortBy_Is_Null()
    {
        var request = new GetAssetsRequest(null, null, null, null, null);

        var result = await _validator.ValidateAsync(request);

        Assert.True(result.IsValid);
    }

    // ─── SortDirection ────────────────────────────────────
    [Theory]
    [InlineData("asc")]
    [InlineData("desc")]
    public async Task Validate_Should_Pass_When_SortDirection_Is_Valid(string sortDirection)
    {
        var request = new GetAssetsRequest(null, null, null, null, sortDirection);

        var result = await _validator.ValidateAsync(request);

        Assert.True(result.IsValid);
    }

    [Fact]
    public async Task Validate_Should_Return_Error_When_SortDirection_Is_Invalid()
    {
        var request = new GetAssetsRequest(null, null, null, null, "invalidDirection");

        var result = await _validator.ValidateAsync(request);

        Assert.False(result.IsValid);
        Assert.Contains(result.Errors,
            x => x.PropertyName == "SortDirection");
    }

    // ─── States ───────────────────────────────────────────
    [Fact]
    public async Task Validate_Should_Pass_When_States_Are_Valid()
    {
        var request = new GetAssetsRequest(null, new[] { "Available", "Assigned" }, null, null, null);

        var result = await _validator.ValidateAsync(request);

        Assert.True(result.IsValid);
    }

    [Fact]
    public async Task Validate_Should_Return_Error_When_State_Is_Invalid()
    {
        var request = new GetAssetsRequest(null, ["InvalidState"], null, null, null);

        var result = await _validator.ValidateAsync(request);

        Assert.False(result.IsValid);
        Assert.Contains(result.Errors,
            x => x.PropertyName == "States[0]");
    }

    // ─── Categories ───────────────────────────────────────
    [Fact]
    public async Task Validate_Should_Pass_When_Category_Exists()
    {
        var request = new GetAssetsRequest(["Laptop"], null, null, null, null);

        var result = await _validator.ValidateAsync(request);

        Assert.True(result.IsValid);
    }

    [Fact]
    public async Task Validate_Should_Return_Error_When_Category_Does_Not_Exist()
    {
        _categoryRepositoryMock
            .Setup(r => r.AnyAsync(
                It.IsAny<CategoryByNameSpec>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);

        var request = new GetAssetsRequest(["Unicorn"], null, null, null, null);

        var result = await _validator.ValidateAsync(request);

        Assert.False(result.IsValid);
        Assert.Contains(result.Errors,
            x => x.ErrorMessage == "Category 'Unicorn' does not exist.");
    }

    // ─── Pagination ───────────────────────────────────────
    [Fact]
    public async Task Validate_Should_Return_Error_When_PageNumber_Is_Less_Than_One()
    {
        var request = new GetAssetsRequest(null, null, null, null, null, PageNumber: 0);

        var result = await _validator.ValidateAsync(request);

        Assert.False(result.IsValid);
        Assert.Contains(result.Errors,
            x => x.ErrorMessage == "PageNumber must be at least 1.");
    }

    [Fact]
    public async Task Validate_Should_Return_Error_When_PageSize_Exceeds_Limit()
    {
        var request = new GetAssetsRequest(null, null, null, null, null, PageSize: 100);

        var result = await _validator.ValidateAsync(request);

        Assert.False(result.IsValid);
        Assert.Contains(result.Errors,
            x => x.ErrorMessage == "PageSize must be between 1 and 50.");
    }
}