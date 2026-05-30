using FluentValidation.TestHelper;
using NashAssetManagement.Application.UseCases.Assets.ViewDetail;
using Xunit;

namespace NashAssetManagement.UnitTests.Application.UseCases.Assets.ViewDetail;

public class GetAssetDetailValidatorTests
{
    private readonly GetAssetDetailValidator _validator;

    public GetAssetDetailValidatorTests()
    {
        _validator = new GetAssetDetailValidator();
    }

    [Fact]
    public async Task Validate_ShouldFail_WhenIdIsEmpty()
    {
        var request = new GetAssetDetailRequest(String.Empty);

        var result = await _validator.ValidateAsync(request);

        Assert.False(result.IsValid);

        Assert.Contains(
            result.Errors,
            e => e.ErrorMessage == "Asset id is required");
    }

    [Fact]
    public async Task Validate_ShouldPass_WhenIdIsValid()
    {
        var request = new GetAssetDetailRequest("A0000000-0000-0000-0000-000000000001");

        var result = await _validator.ValidateAsync(request);

        Assert.True(result.IsValid);
    }

    [Fact]
    public async Task Validate_ShouldFail_WhenIdIsNotGuidFormat()
    {
        var request = new GetAssetDetailRequest("asdfewfwef");

        var result = await _validator.ValidateAsync(request);

        Assert.False(result.IsValid);

        Assert.Contains(
            result.Errors,
            e => e.ErrorMessage == "Asset id must be in Guid Format.");
    }
}