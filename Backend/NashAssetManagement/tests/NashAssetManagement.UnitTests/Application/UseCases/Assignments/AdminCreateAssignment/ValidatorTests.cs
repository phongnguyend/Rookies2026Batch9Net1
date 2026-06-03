using FluentValidation.TestHelper;
using NashAssetManagement.Application.UseCases.Assignments.AdminCreateAssignment;
using Xunit;

namespace NashAssetManagement.UnitTests.Application.UseCases.Assignments.AdminCreateAssignment
{
    public class ValidatorTests
    {
        private readonly Validator _validator;

        public ValidatorTests()
        {
            _validator = new Validator();
        }

        // -------------------------------------------------------------------------
        // Valid Request
        // -------------------------------------------------------------------------

        [Fact]
        public void Validate_ShouldNotHaveAnyValidationErrors_WhenRequestIsValid()
        {
            // Arrange
            var request = new Request(
                UserId: Guid.NewGuid().ToString(),
                AssetId: Guid.NewGuid().ToString(),
                AssignedDate: DateTime.Today,
                Note: "Valid note");

            // Act
            var result = _validator.TestValidate(request);

            // Assert
            result.ShouldNotHaveAnyValidationErrors();
        }

        [Fact]
        public void Validate_ShouldNotHaveAnyValidationErrors_WhenNoteIsNull()
        {
            // Arrange
            var request = new Request(
                UserId: Guid.NewGuid().ToString(),
                AssetId: Guid.NewGuid().ToString(),
                AssignedDate: DateTime.Today,
                Note: null);

            // Act
            var result = _validator.TestValidate(request);

            // Assert
            result.ShouldNotHaveAnyValidationErrors();
        }

        // -------------------------------------------------------------------------
        // UserId Validation
        // -------------------------------------------------------------------------

        [Fact]
        public void Validate_ShouldNotHaveValidationError_WhenUserIdIsValidGuid()
        {
            // Arrange
            var request = new Request(
                UserId: Guid.NewGuid().ToString(),
                AssetId: Guid.NewGuid().ToString(),
                AssignedDate: DateTime.Today,
                Note: null);

            // Act
            var result = _validator.TestValidate(request);

            // Assert
            result.ShouldNotHaveValidationErrorFor(x => x.UserId);
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("   ")]
        public void Validate_ShouldHaveRequiredValidationError_WhenUserIdIsEmptyOrNull(string? invalidId)
        {
            // Arrange
            var request = new Request(
                UserId: invalidId,
                AssetId: Guid.NewGuid().ToString(),
                AssignedDate: DateTime.Today,
                Note: null);

            // Act
            var result = _validator.TestValidate(request);

            // Assert
            result.ShouldHaveValidationErrorFor(x => x.UserId)
                  .WithErrorMessage("User Id is required.");
        }

        [Fact]
        public void Validate_ShouldHaveValidationError_WhenUserIdIsNotValidGuid()
        {
            // Arrange
            var request = new Request(
                UserId: "not-a-valid-guid",
                AssetId: Guid.NewGuid().ToString(),
                AssignedDate: DateTime.Today,
                Note: null);

            // Act
            var result = _validator.TestValidate(request);

            // Assert
            result.ShouldHaveValidationErrorFor(x => x.UserId)
                  .WithErrorMessage("User Id must be a valid Guid/uuid.");
        }

        // -------------------------------------------------------------------------
        // AssetId Validation
        // -------------------------------------------------------------------------

        [Fact]
        public void Validate_ShouldNotHaveValidationError_WhenAssetIdIsValidGuid()
        {
            // Arrange
            var request = new Request(
                UserId: Guid.NewGuid().ToString(),
                AssetId: Guid.NewGuid().ToString(),
                AssignedDate: DateTime.Today,
                Note: null);

            // Act
            var result = _validator.TestValidate(request);

            // Assert
            result.ShouldNotHaveValidationErrorFor(x => x.AssetId);
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("   ")]
        public void Validate_ShouldHaveRequiredValidationError_WhenAssetIdIsEmptyOrNull(string? invalidId)
        {
            // Arrange
            var request = new Request(
                UserId: Guid.NewGuid().ToString(),
                AssetId: invalidId,
                AssignedDate: DateTime.Today,
                Note: null);

            // Act
            var result = _validator.TestValidate(request);

            // Assert
            result.ShouldHaveValidationErrorFor(x => x.AssetId)
                  .WithErrorMessage("Asset Id is required.");
        }

        [Fact]
        public void Validate_ShouldHaveValidationError_WhenAssetIdIsNotValidGuid()
        {
            // Arrange
            var request = new Request(
                UserId: Guid.NewGuid().ToString(),
                AssetId: "not-a-valid-guid",
                AssignedDate: DateTime.Today,
                Note: null);

            // Act
            var result = _validator.TestValidate(request);

            // Assert
            result.ShouldHaveValidationErrorFor(x => x.AssetId)
                  .WithErrorMessage("Asset Id must be a valid Guid/uuid.");
        }

        // -------------------------------------------------------------------------
        // Note Validation
        // -------------------------------------------------------------------------

        [Fact]
        public void Validate_ShouldNotHaveValidationError_WhenNoteIsExactly1000Characters()
        {
            // Arrange
            var request = new Request(
                UserId: Guid.NewGuid().ToString(),
                AssetId: Guid.NewGuid().ToString(),
                AssignedDate: DateTime.Today,
                Note: new string('a', 1000));

            // Act
            var result = _validator.TestValidate(request);

            // Assert
            result.ShouldNotHaveValidationErrorFor(x => x.Note);
        }

        [Fact]
        public void Validate_ShouldHaveValidationError_WhenNoteExceeds1000Characters()
        {
            // Arrange
            var request = new Request(
                UserId: Guid.NewGuid().ToString(),
                AssetId: Guid.NewGuid().ToString(),
                AssignedDate: DateTime.Today,
                Note: new string('a', 1001));

            // Act
            var result = _validator.TestValidate(request);

            // Assert
            result.ShouldHaveValidationErrorFor(x => x.Note)
                  .WithErrorMessage("Note cannot exceed 1000 characters.");
        }

        // -------------------------------------------------------------------------
        // AssignedDate Validation
        // -------------------------------------------------------------------------

        [Fact]
        public void Validate_ShouldNotHaveValidationError_WhenAssignedDateIsToday()
        {
            // Arrange
            var request = new Request(
                UserId: Guid.NewGuid().ToString(),
                AssetId: Guid.NewGuid().ToString(),
                AssignedDate: DateTime.Today,
                Note: null);

            // Act
            var result = _validator.TestValidate(request);

            // Assert
            result.ShouldNotHaveValidationErrorFor(x => x.AssignedDate);
        }

        [Fact]
        public void Validate_ShouldNotHaveValidationError_WhenAssignedDateIsInTheFuture()
        {
            // Arrange
            var request = new Request(
                UserId: Guid.NewGuid().ToString(),
                AssetId: Guid.NewGuid().ToString(),
                AssignedDate: DateTime.Today.AddDays(7),
                Note: null);

            // Act
            var result = _validator.TestValidate(request);

            // Assert
            result.ShouldNotHaveValidationErrorFor(x => x.AssignedDate);
        }

        [Fact]
        public void Validate_ShouldHaveValidationError_WhenAssignedDateIsInThePast()
        {
            // Arrange
            var request = new Request(
                UserId: Guid.NewGuid().ToString(),
                AssetId: Guid.NewGuid().ToString(),
                AssignedDate: DateTime.Today.AddDays(-1),
                Note: null);

            // Act
            var result = _validator.TestValidate(request);

            // Assert
            result.ShouldHaveValidationErrorFor(x => x.AssignedDate)
                  .WithErrorMessage("Assigned date must be current date or in the future.");
        }
    }
}
