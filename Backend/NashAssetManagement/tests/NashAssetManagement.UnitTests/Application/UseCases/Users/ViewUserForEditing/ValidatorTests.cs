using FluentValidation.TestHelper;
using NashAssetManagement.Application.UseCases.Users.ViewUserForEditing;
using Xunit;

namespace NashAssetManagement.UnitTests.Application.UseCases.Users.ViewUserForEditing
{
    public class ValidatorTests
    {
        private readonly Validators _validator = new();

        [Fact]
        public void TestValidate_ShouldHaveValidationErrorForUserId_WhenUserIdIsNull()
        {
            var request = new Request(null);

            var result = _validator.TestValidate(request);

            result.ShouldHaveValidationErrorFor(x => x.UserId);
            Assert.Contains(result.Errors,
                x => x.PropertyName == nameof(Request.UserId)
                     && x.ErrorMessage == "User Id is required.");
        }

        [Fact]
        public void TestValidate_ShouldHaveValidationErrorForUserId_WhenUserIdIsEmpty()
        {
            var request = new Request("");

            var result = _validator.TestValidate(request);

            result.ShouldHaveValidationErrorFor(x => x.UserId);
            Assert.Contains(result.Errors,
                x => x.PropertyName == nameof(Request.UserId)
                     && x.ErrorMessage == "User Id is required.");
        }

        [Fact]
        public void TestValidate_ShouldHaveValidationErrorForUserId_WhenUserIdIsWhitespace()
        {
            var request = new Request("   ");

            var result = _validator.TestValidate(request);

            result.ShouldHaveValidationErrorFor(x => x.UserId);
            Assert.Contains(result.Errors,
                x => x.PropertyName == nameof(Request.UserId)
                     && x.ErrorMessage == "User Id is required.");
        }

        [Fact]
        public void TestValidate_ShouldHaveValidationErrorForUserId_WhenUserIdIsNotGuid()
        {
            var request = new Request("not-a-guid");

            var result = _validator.TestValidate(request);

            result.ShouldHaveValidationErrorFor(x => x.UserId);
            Assert.Single(result.Errors);
            Assert.Equal(nameof(Request.UserId), result.Errors[0].PropertyName);
            Assert.Equal("User Id must be a valid Guid/uuid.", result.Errors[0].ErrorMessage);
        }

        [Fact]
        public void TestValidate_ShouldNotHaveAnyValidationErrors_WhenRequestIsValid()
        {
            var request = new Request("36c29308-4d9c-4e1b-9baf-a5dc11f26001");

            var result = _validator.TestValidate(request);

            result.ShouldNotHaveAnyValidationErrors();
        }
    }
}
