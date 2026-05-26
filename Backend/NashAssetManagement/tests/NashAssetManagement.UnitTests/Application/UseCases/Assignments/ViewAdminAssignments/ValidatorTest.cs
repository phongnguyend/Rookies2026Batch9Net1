using NashAssetManagement.Application.UseCases.Assignments.GetAll;
using NashAssetManagement.Domain.Enums;
using Xunit;

namespace NashAssetManagement.UnitTests.Application.UseCases.Assignments.ViewAdminAssignments
{
    public class ValidatorTest
    {
        private readonly Validator _validator = new();

        // ── Null / Empty ──────────────────────────────────────────

        [Fact]
        public async Task ValidateAsync_NullState_ReturnsValid()
        {
            // Arrange
            var query = new Query { State = null };

            // Act
            var result = await _validator.ValidateAsync(query);

            // Assert
            Assert.True(result.IsValid);
        }

        [Fact]
        public async Task ValidateAsync_EmptyStateArray_ReturnsValid()
        {
            // Arrange
            var query = new Query { State = Array.Empty<string>() };

            // Act
            var result = await _validator.ValidateAsync(query);

            // Assert
            Assert.True(result.IsValid);
        }

        // ── Valid states ──────────────────────────────────────────

        [Fact]
        public async Task ValidateAsync_AllValidStates_ReturnsValid()
        {
            // Arrange
            var query = new Query { State = new[] { "Accepted", "WaitingForAcceptance", "Declined" } };

            // Act
            var result = await _validator.ValidateAsync(query);

            // Assert
            Assert.True(result.IsValid);
        }

        [Fact]
        public async Task ValidateAsync_ValidStatesCaseInsensitive_ReturnsValid()
        {
            // Arrange
            var query = new Query { State = new[] { "accepted", "WAITINGFORACCEPTANCE", "Declined" } };

            // Act
            var result = await _validator.ValidateAsync(query);

            // Assert
            Assert.True(result.IsValid);
        }

        [Fact]
        public async Task ValidateAsync_AllEnumNamesAsStrings_ReturnsValid()
        {
            // Arrange
            var allStates = Enum.GetNames<AssignmentState>();
            var query = new Query { State = allStates };

            // Act
            var result = await _validator.ValidateAsync(query);

            // Assert
            Assert.True(result.IsValid);
        }

        // ── Invalid states ────────────────────────────────────────

        [Fact]
        public async Task ValidateAsync_SingleInvalidState_ReturnsInvalid()
        {
            // Arrange
            var query = new Query { State = new[] { "InvalidState" } };

            // Act
            var result = await _validator.ValidateAsync(query);

            // Assert
            Assert.False(result.IsValid);
        }

        [Fact]
        public async Task ValidateAsync_SingleInvalidState_ErrorMessageContainsStateName()
        {
            // Arrange
            var query = new Query { State = new[] { "InvalidState" } };

            // Act
            var result = await _validator.ValidateAsync(query);

            // Assert
            Assert.Contains(result.Errors,
                e => e.ErrorMessage.Contains("'InvalidState' is not a valid state."));
        }

        [Fact]
        public async Task ValidateAsync_SingleInvalidState_ErrorMessageContainsAllValidValues()
        {
            // Arrange
            var query = new Query { State = new[] { "Bogus" } };

            // Act
            var result = await _validator.ValidateAsync(query);

            // Assert
            var error = Assert.Single(result.Errors);
            foreach (var name in Enum.GetNames<AssignmentState>())
                Assert.Contains(name, error.ErrorMessage);
        }

        [Fact]
        public async Task ValidateAsync_MultipleInvalidStates_ReturnsOneErrorPerInvalidState()
        {
            // Arrange
            var query = new Query { State = new[] { "BadState1", "BadState2" } };

            // Act
            var result = await _validator.ValidateAsync(query);

            // Assert
            Assert.False(result.IsValid);
            Assert.Contains(result.Errors, e => e.ErrorMessage.Contains("'BadState1'"));
            Assert.Contains(result.Errors, e => e.ErrorMessage.Contains("'BadState2'"));
            Assert.Equal(2, result.Errors.Count);
        }

        [Fact]
        public async Task ValidateAsync_MixedValidAndInvalidStates_ReturnsErrorOnlyForInvalid()
        {
            // Arrange
            var query = new Query { State = new[] { "Accepted", "NotAState" } };

            // Act
            var result = await _validator.ValidateAsync(query);

            // Assert
            Assert.False(result.IsValid);
            Assert.Contains(result.Errors, e => e.ErrorMessage.Contains("'NotAState'"));
            Assert.DoesNotContain(result.Errors, e => e.ErrorMessage.Contains("'Accepted'"));
        }

        [Fact]
        public async Task ValidateAsync_EmptyStringState_ReturnsInvalid()
        {
            // Arrange
            var query = new Query { State = new[] { "" } };

            // Act
            var result = await _validator.ValidateAsync(query);

            // Assert
            Assert.False(result.IsValid);
        }

        [Fact]
        public async Task ValidateAsync_WhitespaceState_ReturnsInvalid()
        {
            // Arrange
            var query = new Query { State = new[] { "   " } };

            // Act
            var result = await _validator.ValidateAsync(query);

            // Assert
            Assert.False(result.IsValid);
        }
    }
}
