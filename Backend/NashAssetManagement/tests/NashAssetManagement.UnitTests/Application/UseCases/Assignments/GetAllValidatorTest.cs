using NashAssetManagement.Application.UseCases.Assignments.GetAll;
using Xunit;

namespace NashAssetManagement.UnitTests.Application.UseCases.Assignments
{
    public class GetAllValidatorTest
    {
        private readonly Validator _validator = new();

        [Fact]
        public async Task Validate_Should_Pass_When_State_Is_Empty()
        {
            var query = new Query { State = null };
            var result = await _validator.ValidateAsync(query);
            Assert.True(result.IsValid);
        }

        [Fact]
        public async Task Validate_Should_Pass_When_All_States_Are_Valid()
        {
            var query = new Query { State = new[] { "Accepted", "Waiting", "Declined" } };
            var result = await _validator.ValidateAsync(query);
            Assert.True(result.IsValid);
        }

        [Fact]
        public async Task Validate_Should_Pass_When_States_Are_Valid_Case_Insensitive()
        {
            var query = new Query { State = new[] { "accepted", "WAITING" } };
            var result = await _validator.ValidateAsync(query);
            Assert.True(result.IsValid);
        }

        [Fact]
        public async Task Validate_Should_Return_Error_When_State_Is_Invalid()
        {
            var query = new Query { State = new[] { "InvalidState" } };
            var result = await _validator.ValidateAsync(query);
            Assert.False(result.IsValid);
            Assert.Contains(result.Errors,
                x => x.ErrorMessage.Contains("'InvalidState' is not a valid state."));
        }

        [Fact]
        public async Task Validate_Should_Return_Error_For_Each_Invalid_State()
        {
            var query = new Query { State = new[] { "BadState1", "BadState2" } };
            var result = await _validator.ValidateAsync(query);
            Assert.False(result.IsValid);
            Assert.Contains(result.Errors, x => x.ErrorMessage.Contains("'BadState1'"));
            Assert.Contains(result.Errors, x => x.ErrorMessage.Contains("'BadState2'"));
        }

        [Fact]
        public async Task Validate_Should_Return_Error_Only_For_Invalid_State_In_Mixed_List()
        {
            var query = new Query { State = new[] { "Accepted", "NotAState" } };
            var result = await _validator.ValidateAsync(query);
            Assert.False(result.IsValid);
            Assert.Contains(result.Errors, x => x.ErrorMessage.Contains("'NotAState'"));
            Assert.DoesNotContain(result.Errors, x => x.ErrorMessage.Contains("'Accepted'"));
        }
    }
}
