using NashAssetManagement.Application.UseCases.Auth.Login;
using Xunit;

namespace NashAssetManagement.UnitTests.Application.UseCases.Auth.Login;

public class ValidatorTests
{
    private readonly Validator _validator = new();

    #region Username
    [Trait("UT", "Login")]
    [Fact]
    public async Task Should_Return_Error_When_Username_Is_Empty()
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
    public async Task Should_Return_Error_When_Username_Exceeds_Max_Length()
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
    public async Task Should_Return_Error_When_Username_Format_Is_Invalid()
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
    public async Task Should_Return_Error_When_Password_Is_Empty()
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
    public async Task Should_Not_Return_Error_When_Request_Is_Valid()
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