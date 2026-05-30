using NashAssetManagement.Application.UseCases.Assets.Create;
using NashAssetManagement.Domain.Enums;
using Xunit;

namespace NashAssetManagement.UnitTests.Application.UseCases.Assets.Create;

public class ValidatorTests
{
    private readonly CreateAssetValidator _validator = new();

    [Fact]
    public async Task CreateAssetValidator_AssetName_IsEmpty_ShouldReturnError()
    {
        var request = CreateValidRequest() with
        {
            AssetName = string.Empty
        };

        var result = await _validator.ValidateAsync(request);

        Assert.False(result.IsValid);
        Assert.Contains(result.Errors,
            x => x.ErrorMessage == "Asset name is required.");
    }

    [Fact]
    public async Task CreateAssetValidator_AssetName_ExceedsMaxLength_ShouldReturnError()
    {
        var request = CreateValidRequest() with
        {
            AssetName = new string('A', 101)
        };

        var result = await _validator.ValidateAsync(request);

        Assert.False(result.IsValid);
        Assert.Contains(result.Errors,
            x => x.ErrorMessage == "Asset name must not exceed 100 characters.");
    }

    [Fact]
    public async Task CreateAssetValidator_Specification_IsEmpty_ShouldReturnError()
    {
        var request = CreateValidRequest() with
        {
            Specification = string.Empty
        };

        var result = await _validator.ValidateAsync(request);

        Assert.False(result.IsValid);
        Assert.Contains(result.Errors,
            x => x.ErrorMessage == "Specification is required.");
    }

    [Fact]
    public async Task CreateAssetValidator_Specification_ExceedsMaxLength_ShouldReturnError()
    {
        var request = CreateValidRequest() with
        {
            Specification = new string('A', 501)
        };

        var result = await _validator.ValidateAsync(request);

        Assert.False(result.IsValid);
        Assert.Contains(result.Errors,
            x => x.ErrorMessage == "Specification must not exceed 500 characters.");
    }

    [Fact]
    public async Task CreateAssetValidator_InstalledDate_IsFuture_ShouldReturnError()
    {
        var request = CreateValidRequest() with
        {
            InstalledDate = DateTime.UtcNow.AddDays(1)
        };

        var result = await _validator.ValidateAsync(request);

        Assert.False(result.IsValid);
        Assert.Contains(result.Errors,
            x => x.ErrorMessage == "Installed date cannot be in the future.");
    }

    [Fact]
    public async Task CreateAssetValidator_State_IsInvalid_ShouldReturnError()
    {
        var request = CreateValidRequest() with
        {
            State = AssetState.WaitingForRecycling // invalid by rule
        };

        var result = await _validator.ValidateAsync(request);

        Assert.False(result.IsValid);
        Assert.Contains(result.Errors,
            x => x.ErrorMessage == "State must be Available or NotAvailable.");
    }

    [Fact]
    public async Task CreateAssetValidator_CategoryId_IsEmpty_ShouldStillBeValid()
    {
        // NOTE: you currently have NO rule for CategoryId in validator
        var request = CreateValidRequest() with
        {
            CategoryId = Guid.Empty
        };

        var result = await _validator.ValidateAsync(request);

        Assert.True(result.IsValid); // because no rule exists yet
    }

    [Fact]
    public async Task CreateAssetValidator_Request_IsValid_ShouldPass()
    {
        var request = CreateValidRequest();

        var result = await _validator.ValidateAsync(request);

        Assert.True(result.IsValid);
    }

    private static CreateAssetRequest CreateValidRequest()
    {
        return new CreateAssetRequest(
            AssetName: "Laptop Dell",
            Specification: "Core i7 16GB RAM",
            InstalledDate: DateTime.UtcNow.AddDays(-1),
            State: AssetState.Available,
            CategoryId: Guid.NewGuid()
        );
    }
}