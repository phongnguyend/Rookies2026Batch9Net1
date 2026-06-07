using FluentValidation.TestHelper;
using Moq;
using NashAssetManagement.Application.Abstractions.DateTimes;
using NashAssetManagement.Application.UseCases.Assignments.AdminEditAssignment;
using Xunit;

namespace NashAssetManagement.UnitTests.Application.UseCases.Assignments.AdminEditAssignment
{
    public class ValidatorTests
    {
        readonly Mock<IDateTimeProvider> _mockDateTimeProvider;
        readonly Validator _validator;
        readonly DateTime _fakeToday;

        public ValidatorTests()
        {
            _mockDateTimeProvider = new Mock<IDateTimeProvider>();
            _fakeToday = new DateTime(2026, 6, 4, 10, 0, 0, DateTimeKind.Utc);
            _mockDateTimeProvider.Setup(d => d.UtcNow).Returns(_fakeToday);

            _validator = new Validator(_mockDateTimeProvider.Object);
        }

        [Fact]
        public void Validate_WhenAllFieldsAreValid_ShouldNotHaveAnyValidationErrors()
        {
            // Arrange
            var request = new Request(
                AssignmentId: Guid.NewGuid().ToString(),
                Payload: new AssignmentEditPayload(
                    UserId: Guid.NewGuid().ToString(),
                    AssetId: Guid.NewGuid().ToString(),
                    AssignedDate: _fakeToday.AddDays(1), // Future date
                    Note: "Valid short note description."
                )
            );

            // Act
            var result = _validator.TestValidate(request);

            // Assert
            result.ShouldNotHaveAnyValidationErrors();
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("   ")]
        [InlineData("not-a-guid")]
        public void Validate_WhenAssignmentIdIsInvalid_ShouldHaveValidationError(string? invalidAssignmentId)
        {
            // Arrange
            var request = new Request(AssignmentId: invalidAssignmentId, Payload: null);

            // Act
            var result = _validator.TestValidate(request);

            // Assert
            result.ShouldHaveValidationErrorFor(x => x.AssignmentId);
        }

        [Fact]
        public void Validate_WhenPayloadIsNull_ShouldHaveRequiredErrorMessage()
        {
            // Arrange
            var request = new Request(AssignmentId: Guid.NewGuid().ToString(), Payload: null);

            // Act
            var result = _validator.TestValidate(request);

            // Assert
            result.ShouldHaveValidationErrorFor(x => x.Payload)
                  .WithErrorMessage("Assignment data is required for editing.");
        }

        [Theory]
        [InlineData(null)]
        [InlineData("invalid-user-guid")]
        public void Validate_WhenPayloadUserIdIsInvalid_ShouldHaveValidationError(string? invalidUserId)
        {
            // Arrange
            var request = new Request(
                AssignmentId: Guid.NewGuid().ToString(),
                Payload: new AssignmentEditPayload(
                    UserId: invalidUserId,
                    AssetId: Guid.NewGuid().ToString(),
                    AssignedDate: _fakeToday,
                    Note: null
                )
            );

            // Act
            var result = _validator.TestValidate(request);

            // Assert
            result.ShouldHaveValidationErrorFor(x => x.Payload!.UserId);
        }

        [Theory]
        [InlineData(null)]
        [InlineData("invalid-asset-guid")]
        public void Validate_WhenPayloadAssetIdIsInvalid_ShouldHaveValidationError(string? invalidAssetId)
        {
            // Arrange
            var request = new Request(
                AssignmentId: Guid.NewGuid().ToString(),
                Payload: new AssignmentEditPayload(
                    UserId: Guid.NewGuid().ToString(),
                    AssetId: invalidAssetId,
                    AssignedDate: _fakeToday,
                    Note: null
                )
            );

            // Act
            var result = _validator.TestValidate(request);

            // Assert
            result.ShouldHaveValidationErrorFor(x => x.Payload!.AssetId);
        }

        [Fact]
        public void Validate_WhenNoteExceedsMaxLength_ShouldHaveLengthErrorMessage()
        {
            // Arrange
            var longNote = new string('A', 1001); // 1001 characters long
            var request = new Request(
                AssignmentId: Guid.NewGuid().ToString(),
                Payload: new AssignmentEditPayload(
                    UserId: Guid.NewGuid().ToString(),
                    AssetId: Guid.NewGuid().ToString(),
                    AssignedDate: _fakeToday,
                    Note: longNote
                )
            );

            // Act
            var result = _validator.TestValidate(request);

            // Assert
            result.ShouldHaveValidationErrorFor(x => x.Payload!.Note)
                  .WithErrorMessage("Note cannot exceed 1000 characters.");
        }

        [Fact]
        public void Validate_WhenAssignedDateIsInThePast_ShouldHaveDateErrorMessage()
        {
            // Arrange
            var pastDate = _fakeToday.AddDays(-1); // Yesterday
            var request = new Request(
                AssignmentId: Guid.NewGuid().ToString(),
                Payload: new AssignmentEditPayload(
                    UserId: Guid.NewGuid().ToString(),
                    AssetId: Guid.NewGuid().ToString(),
                    AssignedDate: pastDate,
                    Note: "Note"
                )
            );

            // Act
            var result = _validator.TestValidate(request);

            // Assert
            result.ShouldHaveValidationErrorFor(x => x.Payload!.AssignedDate)
                  .WithErrorMessage("Assigned date must be current date or in the future.");
        }

        [Theory]
        [InlineData(0)] // Exactly today
        [InlineData(5)] // Future date
        public void Validate_WhenAssignedDateIsCurrentOrFuture_ShouldNotHaveDateValidationError(int daysToAdd)
        {
            // Arrange
            var validDate = _fakeToday.AddDays(daysToAdd);
            var request = new Request(
                AssignmentId: Guid.NewGuid().ToString(),
                Payload: new AssignmentEditPayload(
                    UserId: Guid.NewGuid().ToString(),
                    AssetId: Guid.NewGuid().ToString(),
                    AssignedDate: validDate,
                    Note: null
                )
            );

            // Act
            var result = _validator.TestValidate(request);

            // Assert
            result.ShouldNotHaveValidationErrorFor(x => x.Payload!.AssignedDate);
        }
    }
}
