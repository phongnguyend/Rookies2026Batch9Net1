using FluentValidation.TestHelper;
using NashAssetManagement.Application.UseCases.Users.Disable;
using Xunit;

namespace NashAssetManagement.UnitTests.Application.UseCases.Users.Disable
{
    public class ValidatorTests
    {
        private readonly Validator _validator;

        public ValidatorTests()
        {
            _validator = new Validator();
        }

        [Fact]
        public void TestValidate_TargetUserIdIsNotGuid_ValidationErrorForTargetUserId()
        {
            // Arrange
            var request = new Request("not-a-guid");

            // Act
            var result = _validator.TestValidate(request);

            // Assert
            result.ShouldHaveValidationErrorFor(x => x.TargetUserId)
                .WithErrorMessage("Target User Id must be a valid Guid/uuid.");
        }

        [Fact]
        public void TestValidate_RequestIsValid_NoValidationErrors()
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
