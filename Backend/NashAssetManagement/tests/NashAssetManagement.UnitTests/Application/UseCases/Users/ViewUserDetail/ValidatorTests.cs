using FluentValidation.TestHelper;
using NashAssetManagement.Application.UseCases.Users.ViewUserDetail;
using Xunit;

namespace NashAssetManagement.UnitTests.Application.UseCases.Users.ViewUserDetail
{
    public class ValidatorTests
    {
        private readonly Validators _validator;

        public ValidatorTests()
        {
            _validator = new Validators();
        }

        [Fact]
        public void TestValidate_ShouldHaveValidationErrorForUserId_WhenUserIdIsNull()
        {
            // Arrange
            var request = new Request(null);

            // Act
            var result = _validator.TestValidate(request);

            // Assert
            result.ShouldHaveValidationErrorFor(x => x.UserId);
            Assert.Contains(result.Errors,
                x => x.PropertyName == nameof(Request.UserId)
                     && x.ErrorMessage == "User id is required.");
        }

        [Fact]
        public void TestValidate_ShouldHaveValidationErrorForUserId_WhenUserIdIsEmpty()
        {
            // Arrange
            var request = new Request("");

            // Act
            var result = _validator.TestValidate(request);

            // Assert
            result.ShouldHaveValidationErrorFor(x => x.UserId);
            Assert.Contains(result.Errors,
                x => x.PropertyName == nameof(Request.UserId)
                     && x.ErrorMessage == "User id is required.");
        }

        [Fact]
        public void TestValidate_ShouldHaveValidationErrorForUserId_WhenUserIdIsWhitespace()
        {
            // Arrange
            var request = new Request("   ");

            // Act
            var result = _validator.TestValidate(request);

            // Assert
            result.ShouldHaveValidationErrorFor(x => x.UserId);
            Assert.Contains(result.Errors,
                x => x.PropertyName == nameof(Request.UserId)
                     && x.ErrorMessage == "User id is required.");
        }

        [Fact]
        public void TestValidate_ShouldHaveValidationErrorForUserId_WhenUserIdIsNotGuid()
        {
            // Arrange
            var request = new Request("not-a-guid");

            // Act
            var result = _validator.TestValidate(request);

            // Assert
            result.ShouldHaveValidationErrorFor(x => x.UserId);
            Assert.Single(result.Errors);
            var error = result.Errors[0];
            Assert.Equal(nameof(Request.UserId), error.PropertyName);
            Assert.Equal("User id must be a valid GUID.", error.ErrorMessage);
        }

        [Fact]
        public void TestValidate_ShouldNotHaveAnyValidationErrors_WhenRequestIsValid()
        {
            // Arrange
            var request = new Request("36c29308-4d9c-4e1b-9baf-a5dc11f26001");

            // Act
            var result = _validator.TestValidate(request);

            // Assert
            result.ShouldNotHaveAnyValidationErrors();
        }
    }
}
