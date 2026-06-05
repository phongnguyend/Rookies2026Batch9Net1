using FluentValidation.TestHelper;
using Xunit;
using NashAssetManagement.Application.UseCases.Assets.Delete;

public class DeleteAssetValidatorTests
{
    private readonly DeleteAssetValidator _validator = new();

    [Fact]
    public void Validate_EmptyId_ShouldReturnError()
    {
        var request = new DeleteAssetRequest("");

        var result = _validator.TestValidate(request);

        result.ShouldHaveValidationErrorFor(x => x.Id)
              .WithErrorMessage("Asset id is required");
    }

    [Fact]
    public void Validate_NullId_ShouldReturnError()
    {
        var request = new DeleteAssetRequest(null!);

        var result = _validator.TestValidate(request);

        result.ShouldHaveValidationErrorFor(x => x.Id)
              .WithErrorMessage("Asset id is required");
    }

    [Fact]
    public void Validate_InvalidGuid_ShouldReturnGuidFormatError()
    {
        var request = new DeleteAssetRequest("invalid-guid");

        var result = _validator.TestValidate(request);

        result.ShouldHaveValidationErrorFor(x => x.Id);
    }

    [Fact]
    public void Validate_ValidGuid_ShouldPass()
    {
        var request = new DeleteAssetRequest(Guid.NewGuid().ToString());

        var result = _validator.TestValidate(request);

        result.ShouldNotHaveAnyValidationErrors();
    }
}