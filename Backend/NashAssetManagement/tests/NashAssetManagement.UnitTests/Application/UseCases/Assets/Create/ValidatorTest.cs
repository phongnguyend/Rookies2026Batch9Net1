using NashAssetManagement.Application.UseCases.Assets.Create;
using NashAssetManagement.Domain.Enums;
using Xunit;

namespace NashAssetManagement.UnitTests.Application.UseCases.Assets.Create;

public class ValidatorTests
{
    private readonly CreateAssetValidator _validator = new();

    [Fact]
    public async Task Validate_Should_Return_Error_When_AssetName_Is_Empty()
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
    public async Task Validate_Should_Return_Error_When_AssetName_Exceeds_Max_Length()
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
    public async Task Validate_Should_Return_Error_When_Specification_Is_Empty()
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
    public async Task Validate_Should_Return_Error_When_Specification_Exceeds_Max_Length()
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
    public async Task Validate_Should_Return_Error_When_InstalledDate_Is_In_Future()
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
    public async Task Validate_Should_Return_Error_When_State_Is_Invalid()
    {
        var request = CreateValidRequest() with
        {
            State = AssetState.WaitingForRecycling
        };

        var result = await _validator.ValidateAsync(request);

        Assert.False(result.IsValid);
        Assert.Contains(result.Errors,
            x => x.ErrorMessage == "State must be Available or NotAvailable.");
    }

    [Fact]
    public async Task Validate_Should_Return_Error_When_CategoryName_Is_Empty()
    {
        var request = CreateValidRequest() with
        {
            CategoryName = string.Empty
        };

        var result = await _validator.ValidateAsync(request);

        Assert.False(result.IsValid);
        Assert.Contains(result.Errors,
            x => x.ErrorMessage == "Category name is required.");
    }

    [Fact]
    public async Task Validate_Should_Return_Error_When_CategoryName_Exceeds_Max_Length()
    {
        var request = CreateValidRequest() with
        {
            CategoryName = new string('A', 21)
        };

        var result = await _validator.ValidateAsync(request);

        Assert.False(result.IsValid);
        Assert.Contains(result.Errors,
            x => x.ErrorMessage == "Category name must not exceed 20 characters.");
    }

    [Fact]
    public async Task Validate_Should_Return_Error_When_CategoryPrefix_Is_Empty()
    {
        var request = CreateValidRequest() with
        {
            CategoryPrefix = string.Empty
        };

        var result = await _validator.ValidateAsync(request);

        Assert.False(result.IsValid);
        Assert.Contains(result.Errors,
            x => x.ErrorMessage == "Category prefix is required.");
    }

    [Fact]
    public async Task Validate_Should_Return_Error_When_CategoryPrefix_Exceeds_Max_Length()
    {
        var request = CreateValidRequest() with
        {
            CategoryPrefix = "ABC"
        };

        var result = await _validator.ValidateAsync(request);

        Assert.False(result.IsValid);
        Assert.Contains(result.Errors,
            x => x.ErrorMessage == "Category prefix must not exceed 2 characters.");
    }

    [Fact]
    public async Task Validate_Should_Return_Error_When_CategoryPrefix_Is_Not_Uppercase()
    {
        var request = CreateValidRequest() with
        {
            CategoryPrefix = "ab"
        };

        var result = await _validator.ValidateAsync(request);

        Assert.False(result.IsValid);
        Assert.Contains(result.Errors,
            x => x.ErrorMessage == "Category prefix must contain uppercase letters only.");
    }

    [Fact]
    public async Task Validate_Should_Pass_When_Request_Is_Valid()
    {
        var request = CreateValidRequest();

        var result = await _validator.ValidateAsync(request);

        Assert.True(result.IsValid);
    }

    private static CreateAssetRequest CreateValidRequest()
    {
        return new CreateAssetRequest(
            "Laptop Dell",
            "Core i7 16GB RAM",
            DateTime.UtcNow.AddDays(-1),
            AssetState.Available,
            "Laptop",
            "LA");
    }
}