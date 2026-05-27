using System.Threading.Tasks;
using NashAssetManagement.Application.UseCases.Users.ViewUserDetail;
using Xunit;

namespace NashAssetManagement.UnitTests.Application.UseCases.Users.ViewUserDetail
{
    public class ValidatorTests
    {
        private readonly Validators _validator = new();

        [Fact]
        public async Task ViewUserDetailValidator_UserIdIsNull_ShouldReturnErrors()
        {
            var request = new Request(null);

            var result = await _validator.ValidateAsync(request);

            Assert.False(result.IsValid);
            Assert.Contains(result.Errors,
                x => x.PropertyName == nameof(Request.UserId)
                     && x.ErrorMessage == "User Id is required.");
        }

        [Fact]
        public async Task ViewUserDetailValidator_UserIdIsEmpty_ShouldReturnErrors()
        {
            var request = new Request("");

            var result = await _validator.ValidateAsync(request);

            Assert.False(result.IsValid);
            Assert.Contains(result.Errors,
                x => x.PropertyName == nameof(Request.UserId)
                     && x.ErrorMessage == "User Id is required.");
        }

        [Fact]
        public async Task ViewUserDetailValidator_UserIdIsWhitespace_ShouldReturnErrors()
        {
            var request = new Request("   ");

            var result = await _validator.ValidateAsync(request);

            Assert.False(result.IsValid);
            Assert.Contains(result.Errors,
                x => x.PropertyName == nameof(Request.UserId)
                     && x.ErrorMessage == "User Id is required.");
        }

        [Fact]
        public async Task ViewUserDetailValidator_UserIdIsNotGuid_ShouldReturnErrors()
        {
            var request = new Request("not-a-guid");

            var result = await _validator.ValidateAsync(request);

            Assert.False(result.IsValid);
            Assert.Contains(result.Errors,
                x => x.PropertyName == nameof(Request.UserId)
                     && x.ErrorMessage == "User Id must be a valid Guid/uuid.");
        }

        [Fact]
        public async Task ViewUserDetailValidator_ValidRequest_ShouldPassValidation()
        {
            var request = new Request("36c29308-4d9c-4e1b-9baf-a5dc11f26001");

            var result = await _validator.ValidateAsync(request);

            Assert.True(result.IsValid);
            Assert.Empty(result.Errors);
        }
    }
}
