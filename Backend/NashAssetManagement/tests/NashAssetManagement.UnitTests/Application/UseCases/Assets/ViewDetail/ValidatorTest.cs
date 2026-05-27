using NashAssetManagement.Application.UseCases.Assets.ViewDetail;
using Xunit;

namespace NashAssetManagement.UnitTests.Application.UseCases.Assets.ViewDetail;

public class ValidatorTests
{
    private readonly GetAssetDetailValidator _validator = new();

    [Fact]
    public async Task Validate_Should_Return_Error_When_Id_Is_Empty()
    {

        var request = new GetAssetDetailRequest(string.Empty);

        var result = await _validator.ValidateAsync(request);

        Assert.False(result.IsValid);
        Assert.Contains(result.Errors,
            x => x.ErrorMessage == "Asset id is required.");
    }

    [Fact]
    public async Task Validate_Should_Pass_When_Id_Is_Valid()
    {
        
        var request = new GetAssetDetailRequest(Guid.NewGuid().ToString());

        var result = await _validator.ValidateAsync(request);

        Assert.True(result.IsValid);
    }
}