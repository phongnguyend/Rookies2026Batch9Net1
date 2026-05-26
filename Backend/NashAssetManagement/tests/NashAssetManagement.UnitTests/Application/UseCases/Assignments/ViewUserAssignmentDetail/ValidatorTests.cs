using FluentValidation.TestHelper;
using NashAssetManagement.Application.UseCases.Assignments.ViewUserAssignmentDetail;
using Xunit;

namespace NashAssetManagement.UnitTests.Application.UseCases.Assignments.ViewUserAssignmentDetail
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

        [Fact]
        public void Validate_ShouldHaveFormatValidationError_WhenAssignmentIdIsMalformed()
        {
            // Arrange
            var request = new Request(AssignmentId: "not-a-valid-guid-12345");

            // Act
            var result = _validator.TestValidate(request);

            // Assert
            result.ShouldHaveValidationErrorFor(x => x.AssignmentId)
                  .WithErrorMessage("Assignment Id must be a valid Guid/uuid.");
        }
    }
}
