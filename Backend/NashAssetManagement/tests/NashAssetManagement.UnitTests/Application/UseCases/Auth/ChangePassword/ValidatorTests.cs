
using NashAssetManagement.Application.UseCases.Auth.ChangePassword;
using Xunit;

// Naming conventions: MethodName_StateUnderTest_ExpectedBehavior
namespace NashAssetManagement.UnitTests.Application.UseCases.Auth.ChangePassword
{
    public class ValidatorTests
    {
        private readonly Validator _validator = new();

        [Fact]
        public async Task ChangePasswordValidator_OldPasswordIsEmpty_ShouldReturnErrors()
        {
            var request = new Request("", "NewPassword123!");

            var result = await _validator.ValidateAsync(request);

            Assert.False(result.IsValid);
            Assert.Contains(result.Errors,
                x => x.ErrorMessage == "Old password is required");
        }

        [Fact]
        public async Task ChangePasswordValidator_NewPasswordIsEmpty_ShouldReturnErrors()
        {
            var request = new Request("OldPassword123!", "");

            var result = await _validator.ValidateAsync(request);

            Assert.False(result.IsValid);
            Assert.Contains(result.Errors,
                x => x.ErrorMessage == "New password is required");
        }

        [Fact]
        public async Task ChangePasswordValidator_ValidRequest_ShouldPassValidation()
        {
            var request = new Request("OldPassword123!", "NewPassword123!");

            var result = await _validator.ValidateAsync(request);

            Assert.True(result.IsValid);
            Assert.Empty(result.Errors);
        }

        [Fact]
        public async Task ChangePasswordValidator_OldPasswordAndNewPasswordAreEmpty_ShouldReturnErrors()
        {
            var request = new Request("", "");

            var result = await _validator.ValidateAsync(request);

            Assert.False(result.IsValid);
            Assert.Equal(2, result.Errors.Count);

            Assert.Contains(result.Errors,
                x => x.PropertyName == nameof(Request.OldPassword)
                     && x.ErrorMessage == "Old password is required");

            Assert.Contains(result.Errors,
                x => x.PropertyName == nameof(Request.NewPassword)
                     && x.ErrorMessage == "New password is required");
        }

        [Fact]
        public async Task ChangePasswordValidator_NewPasswordSameAsOldPassword_ShouldReturnErrors()
        {
            var request = new Request("Password123!", "Password123!");

            var result = await _validator.ValidateAsync(request);

            Assert.False(result.IsValid);

            Assert.Contains(result.Errors,
                x => x.PropertyName == nameof(Request.NewPassword)
                    && x.ErrorMessage == "New password must be different from old password");
        }

        [Fact]
        public async Task ChangePasswordValidator_NewPasswordLessThanSixCharacters_ShouldReturnErrors()
        {
            var request = new Request("OldPassword123!", "Ab1!");

            var result = await _validator.ValidateAsync(request);

            Assert.False(result.IsValid);
            Assert.Contains(result.Errors,
                x => x.PropertyName == nameof(Request.NewPassword)
                     && x.ErrorMessage == "New password must be at least 6 characters");
        }

        [Fact]
        public async Task ChangePasswordValidator_NewPasswordWithoutLowercaseLetter_ShouldReturnErrors()
        {
            var request = new Request("OldPassword123!", "PASSWORD123!");

            var result = await _validator.ValidateAsync(request);

            Assert.False(result.IsValid);
            Assert.Contains(result.Errors,
                x => x.PropertyName == nameof(Request.NewPassword)
                     && x.ErrorMessage == "New password must contain at least one lowercase letter");
        }

        [Fact]
        public async Task ChangePasswordValidator_NewPasswordWithoutDigit_ShouldReturnErrors()
        {
            var request = new Request("OldPassword123!", "NewPassword!");

            var result = await _validator.ValidateAsync(request);

            Assert.False(result.IsValid);
            Assert.Contains(result.Errors,
                x => x.PropertyName == nameof(Request.NewPassword)
                     && x.ErrorMessage == "New password must contain at least one digit");
        }

        [Fact]
        public async Task ChangePasswordValidator_NewPasswordWithoutNonAlphanumericCharacter_ShouldReturnErrors()
        {
            var request = new Request("OldPassword123!", "NewPassword123");

            var result = await _validator.ValidateAsync(request);

            Assert.False(result.IsValid);
            Assert.Contains(result.Errors,
                x => x.PropertyName == nameof(Request.NewPassword)
                     && x.ErrorMessage == "New password must contain at least one non-alphanumeric character");
        }
    }
}
