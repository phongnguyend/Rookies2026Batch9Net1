
using NashAssetManagement.Application.UseCases.Auth.ChangePassword;
using Xunit;

namespace NashAssetManagement.UnitTests.Application.UseCases.Auth.ChangePassword
{
    public class ValidatorTests
    {
        private readonly Validator _validator = new();

        [Fact]
        public async Task Validate_Should_Return_Error_When_OldPassword_Is_Empty()
        {
            var request = new Request("", "NewPassword123");

            var result = await _validator.ValidateAsync(request);

            Assert.False(result.IsValid);
            Assert.Contains(result.Errors,
                x => x.ErrorMessage == "Old password is required");
        }

        [Fact]
        public async Task Validate_Should_Return_Error_When_NewPassword_Is_Empty()
        {
            var request = new Request("OldPassword123", "");

            var result = await _validator.ValidateAsync(request);

            Assert.False(result.IsValid);
            Assert.Contains(result.Errors,
                x => x.ErrorMessage == "New password is required");
        }

        [Fact]
        public async Task Validate_Should_Pass_When_Request_Is_Valid()
        {
            var request = new Request("OldPassword123", "NewPassword123");

            var result = await _validator.ValidateAsync(request);

            Assert.True(result.IsValid);
            Assert.Empty(result.Errors);
        }

        [Fact]
        public async Task Validate_Should_Return_Errors_When_OldPassword_And_NewPassword_Are_Empty()
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
    }
}
