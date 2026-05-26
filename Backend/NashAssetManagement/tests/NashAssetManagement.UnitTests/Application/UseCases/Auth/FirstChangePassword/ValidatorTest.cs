using NashAssetManagement.Application.UseCases.Auth.FirstChangePassword;
using Xunit;

namespace NashAssetManagement.UnitTests.Application.UseCases.Auth.FirstChangePassword;

public class ValidatorTests
{
    private readonly Validator _validator = new();

    #region NewPassword
    [Trait("UT", "FirstChangePassword")]
    [Fact]
    public async Task Validator_NewPasswordIsEmpty_ShouldReturnError()
    {
        // Arrange
        var request = new Request("");

        // Act
        var result = await _validator.ValidateAsync(request);

        // Assert
        Assert.False(result.IsValid);

        Assert.Contains(result.Errors, error =>
            error.PropertyName == nameof(Request.NewPassword) &&
            error.ErrorMessage == "New password is required.");
    }

    [Trait("UT", "FirstChangePassword")]
    [Fact]
    public async Task Validator_NewPasswordTooShort_ShouldReturnError()
    {
        // Arrange
        var request = new Request("A1@");

        // Act
        var result = await _validator.ValidateAsync(request);

        // Assert
        Assert.False(result.IsValid);

        Assert.Contains(result.Errors, error =>
            error.PropertyName == nameof(Request.NewPassword) &&
            error.ErrorMessage == "New password must be at least 6 characters long and less than 100 characters.");
    }

    [Trait("UT", "FirstChangePassword")]
    [Fact]
    public async Task Validator_NewPasswordTooLong_ShouldReturnError()
    {
        // Arrange
        var password = new string('A', 101) + "1@";
        var request = new Request(password);

        // Act
        var result = await _validator.ValidateAsync(request);

        // Assert
        Assert.False(result.IsValid);

        Assert.Contains(result.Errors, error =>
            error.PropertyName == nameof(Request.NewPassword) &&
            error.ErrorMessage == "New password must be at least 6 characters long and less than 100 characters.");
    }

    [Trait("UT", "FirstChangePassword")]
    [Fact]
    public async Task Validator_NewPasswordMissingLetter_ShouldReturnError()
    {
        // Arrange
        var request = new Request("123456@");

        // Act
        var result = await _validator.ValidateAsync(request);

        // Assert
        Assert.False(result.IsValid);

        Assert.Contains(result.Errors, error =>
            error.PropertyName == nameof(Request.NewPassword) &&
            error.ErrorMessage == "New password must contain at least one letter, one number, and one @ character.");
    }

    [Trait("UT", "FirstChangePassword")]
    [Fact]
    public async Task Validator_NewPasswordMissingNumber_ShouldReturnError()
    {
        // Arrange
        var request = new Request("Password@");

        // Act
        var result = await _validator.ValidateAsync(request);

        // Assert
        Assert.False(result.IsValid);

        Assert.Contains(result.Errors, error =>
            error.PropertyName == nameof(Request.NewPassword) &&
            error.ErrorMessage == "New password must contain at least one letter, one number, and one @ character.");
    }

    [Trait("UT", "FirstChangePassword")]
    [Fact]
    public async Task Validator_NewPasswordMissingAtCharacter_ShouldReturnError()
    {
        // Arrange
        var request = new Request("Password123");

        // Act
        var result = await _validator.ValidateAsync(request);

        // Assert
        Assert.False(result.IsValid);

        Assert.Contains(result.Errors, error =>
            error.PropertyName == nameof(Request.NewPassword) &&
            error.ErrorMessage == "New password must contain at least one letter, one number, and one @ character.");
    }

    [Trait("UT", "FirstChangePassword")]
    [Fact]
    public async Task Validator_NewPasswordContainsInvalidCharacter_ShouldReturnError()
    {
        // Arrange
        var request = new Request("Password123!");

        // Act
        var result = await _validator.ValidateAsync(request);

        // Assert
        Assert.False(result.IsValid);

        Assert.Contains(result.Errors, error =>
            error.PropertyName == nameof(Request.NewPassword) &&
            error.ErrorMessage == "New password must contain at least one letter, one number, and one @ character.");
    }

    [Trait("UT", "FirstChangePassword")]
    [Fact]
    public async Task Validator_NewPasswordIsValid_ShouldNotReturnError()
    {
        // Arrange
        var request = new Request("Password123@");

        // Act
        var result = await _validator.ValidateAsync(request);

        // Assert
        Assert.True(result.IsValid);
        Assert.Empty(result.Errors);
    }
    #endregion
}