using FluentValidation.TestHelper;
using NashAssetManagement.Application.UseCases.ReturnRequests.UserCreateReturnRequest;
using Xunit;

namespace NashAssetManagement.UnitTests.Application.UseCases.ReturnRequests.UserCreateReturnRequest
{
    public class ValidatorTests
    {
        private readonly Validator _validator;

        public ValidatorTests()
        {
            _validator = new Validator();
        }

        [Fact]
        public void Validate_ShouldNotHaveValidationError_WhenAssignmentIdIsValidGuid()
        {
            // Arrange
            var request = new Request(AssignmentId: Guid.NewGuid().ToString());

            // Act
            var result = _validator.TestValidate(request);

            // Assert
            result.ShouldNotHaveValidationErrorFor(x => x.AssignmentId);
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("   ")]
        public void Validate_ShouldHaveRequiredValidationError_WhenAssignmentIdIsEmptyOrNull(string? invalidId)
        {
            // Arrange
            var request = new Request(AssignmentId: invalidId);

            // Act
            var result = _validator.TestValidate(request);

            // Assert
            result.ShouldHaveValidationErrorFor(x => x.AssignmentId)
                  .WithErrorMessage("Assignment Id is required.");
        }

        [Theory]
        [InlineData("not-a-valid-guid-12345")]
        [InlineData("hello")]
        [InlineData("1")]
        public void Validate_ShouldHaveFormatValidationError_WhenAssignmentIdIsMalformed(string? invalidId)
        {
            // Arrange
            var request = new Request(AssignmentId: invalidId);

            // Act
            var result = _validator.TestValidate(request);

            // Assert
            result.ShouldHaveValidationErrorFor(x => x.AssignmentId)
                  .WithErrorMessage("Assignment Id must be a valid Guid/uuid.");
        }
    }
}
