using NashAssetManagement.Application.UseCases.Assets.Edit;
using NashAssetManagement.Domain.Enums;
using Xunit;

namespace NashAssetManagement.UnitTests.Application.UseCases.Assets.Edit;

public class ValidatorTests
{
    private readonly EditAssetValidator _validator = new();

    private static EditAssetRequest ValidRequest() => new(
        AssetId: Guid.NewGuid().ToString(),
        AssetName: "Laptop Dell XPS",
        Specification: "Core i7, 16GB RAM",
        InstalledDate: DateTime.UtcNow.AddDays(-1),
        State: AssetState.Available
    );

    [Fact]
    public async Task Validate_ValidRequest_ShouldPass()
    {
        var result = await _validator.ValidateAsync(ValidRequest());

        Assert.True(result.IsValid);
    }

    [Fact]
    public async Task Validate_AssetNameIsEmpty_ShouldReturnError()
    {
        var request = ValidRequest() with { AssetName = "" };

        var result = await _validator.ValidateAsync(request);

        Assert.False(result.IsValid);
        Assert.Contains(result.Errors,
            x => x.ErrorMessage == "Asset name is required.");
    }

    [Fact]
    public async Task Validate_AssetNameExceedsMaxLength_ShouldReturnError()
    {
        var request = ValidRequest() with { AssetName = new string('A', 101) };

        var result = await _validator.ValidateAsync(request);

        Assert.False(result.IsValid);
        Assert.Contains(result.Errors,
            x => x.ErrorMessage == "Asset name must not exceed 100 characters.");
    }

    [Fact]
    public async Task Validate_SpecificationIsEmpty_ShouldReturnError()
    {
        var request = ValidRequest() with { Specification = "" };

        var result = await _validator.ValidateAsync(request);

        Assert.False(result.IsValid);
        Assert.Contains(result.Errors,
            x => x.ErrorMessage == "Specification is required.");
    }

    [Fact]
    public async Task Validate_SpecificationExceedsMaxLength_ShouldReturnError()
    {
        var request = ValidRequest() with { Specification = new string('A', 501) };

        var result = await _validator.ValidateAsync(request);

        Assert.False(result.IsValid);
        Assert.Contains(result.Errors,
            x => x.ErrorMessage == "Specification must not exceed 500 characters.");
    }

    [Fact]
    public async Task Validate_InstalledDateIsToday_ShouldPass()
    {
        var request = ValidRequest() with { InstalledDate = DateTime.UtcNow };

        var result = await _validator.ValidateAsync(request);

        Assert.True(result.IsValid);
    }

    [Fact]
    public async Task Validate_InstalledDateIsInPast_ShouldPass()
    {
        var request = ValidRequest() with { InstalledDate = DateTime.UtcNow.AddYears(-1) };

        var result = await _validator.ValidateAsync(request);

        Assert.True(result.IsValid);
    }

    [Fact]
    public async Task Validate_StateIsAssigned_ShouldReturnError()
    {
        var request = ValidRequest() with { State = AssetState.Assigned };

        var result = await _validator.ValidateAsync(request);

        Assert.False(result.IsValid);
        Assert.Contains(result.Errors,
            x => x.ErrorMessage == "State cannot be Assigned.");
    }

    [Theory]
    [InlineData(AssetState.Available)]
    [InlineData(AssetState.NotAvailable)]
    [InlineData(AssetState.WaitingForRecycling)]
    [InlineData(AssetState.Recycled)]
    public async Task Validate_StateIsNotAssigned_ShouldPass(AssetState state)
    {
        var request = ValidRequest() with { State = state };

        var result = await _validator.ValidateAsync(request);

        Assert.True(result.IsValid);
    }
}