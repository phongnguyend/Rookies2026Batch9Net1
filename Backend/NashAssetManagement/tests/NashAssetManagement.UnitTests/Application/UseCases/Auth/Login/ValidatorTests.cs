using NashAssetManagement.Application.UseCases.Auth.Login;
using Xunit;

namespace NashAssetManagement.UnitTests.Application.UseCases.Auth.Login;

public class ValidatorTests
{
    private readonly Validator _validator = new();

    #region Username
    [Trait("UT", "Login")]
    [Fact]
    public async Task Validator_UsernameIsEmpty_ShouldReturnError()
    {
        // Arrange
        var request = new Request("", "password123");

        // Act
        var result = await _validator.ValidateAsync(request);

        // Assert
        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, error =>
            error.PropertyName == nameof(Request.Username) &&
            error.ErrorMessage == Validator.UsernameRequired);
    }

    [Trait("UT", "Login")]
    [Fact]
    public async Task Validator_UsernameExceedsMaxLength_ShouldReturnError()
    {
        // Arrange
        var username = new string('a', 101);
        var request = new Request(username, "password123");

        // Act
        var result = await _validator.ValidateAsync(request);

        // Assert
        Assert.False(result.IsValid);

        Assert.Contains(result.Errors, error =>
            error.PropertyName == nameof(Request.Username) &&
            error.ErrorMessage == "Username must not exceed 100 characters.");
    }

    [Trait("UT", "Login")]
    [Fact]
    public async Task Validator_UsernameFormatInvalid_ShouldReturnError()
    {
        // Arrange
        var request = new Request("duy123", "password123");

        // Act
        var result = await _validator.ValidateAsync(request);

        // Assert
        Assert.False(result.IsValid);

        Assert.Contains(result.Errors, error =>
            error.PropertyName == nameof(Request.Username) &&
            error.ErrorMessage == "Username must contain only letters.");
    }
    #endregion

    #region Password
    [Trait("UT", "Login")]
    [Fact]
    public async Task Validator_PasswordIsEmpty_ShouldReturnError()
    {
        // Arrange
        var request = new Request("duy", "");

        // Act
        var result = await _validator.ValidateAsync(request);

        // Assert
        Assert.False(result.IsValid);

        Assert.Contains(result.Errors, error =>
            error.PropertyName == nameof(Request.Password) &&
            error.ErrorMessage == "Password is required.");
    }
    #endregion

    #region Request
    [Trait("UT", "Login")]
    [Fact]
    public async Task Validator_RequestIsValid_ShouldNotReturnError()
    {
        // Arrange
        var request = new Request("duy", "password123");

        // Act
        var result = await _validator.ValidateAsync(request);

        // Assert
        Assert.True(result.IsValid);
        Assert.Empty(result.Errors);
    }
    #endregion
}