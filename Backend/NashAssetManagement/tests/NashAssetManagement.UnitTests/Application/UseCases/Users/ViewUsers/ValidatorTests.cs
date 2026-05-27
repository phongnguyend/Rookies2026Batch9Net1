using NashAssetManagement.Application.UseCases.Users.ViewUsers;
using Xunit;

namespace NashAssetManagement.UnitTests.Application.UseCases.Users.ViewUsers
{
    public class ValidatorTests
    {
        private readonly Validators _validator = new();

        [Fact]
        public async Task ViewUserListValidator_PageNumberIsZero_ShouldReturnErrors()
        {
            var request = new Request(0, 10, null, null, null, null);

            var result = await _validator.ValidateAsync(request);

            Assert.False(result.IsValid);
            Assert.Contains(result.Errors,
                x => x.PropertyName == nameof(Request.PageNumber)
                     && x.ErrorMessage == "Page number must be greater than 0.");
        }

        [Fact]
        public async Task ViewUserListValidator_PageNumberIsNegative_ShouldReturnErrors()
        {
            var request = new Request(-1, 10, null, null, null, null);

            var result = await _validator.ValidateAsync(request);

            Assert.False(result.IsValid);
            Assert.Contains(result.Errors,
                x => x.PropertyName == nameof(Request.PageNumber)
                     && x.ErrorMessage == "Page number must be greater than 0.");
        }

        [Fact]
        public async Task ViewUserListValidator_PageSizeIsZero_ShouldReturnErrors()
        {
            var request = new Request(1, 0, null, null, null, null);

            var result = await _validator.ValidateAsync(request);

            Assert.False(result.IsValid);
            Assert.Contains(result.Errors,
                x => x.PropertyName == nameof(Request.PageSize)
                     && x.ErrorMessage == "'Page Size' must be greater than '0'.");
        }

        [Fact]
        public async Task ViewUserListValidator_PageSizeIsGreaterThanMax_ShouldReturnErrors()
        {
            var request = new Request(1, 101, null, null, null, null);

            var result = await _validator.ValidateAsync(request);

            Assert.False(result.IsValid);
            Assert.Contains(result.Errors,
                x => x.PropertyName == nameof(Request.PageSize)
                     && x.ErrorMessage == "Page size must be between 0 and 100.");
        }

        [Fact]
        public async Task ViewUserListValidator_PageNumberAndPageSizeAreInvalid_ShouldReturnErrors()
        {
            var request = new Request(0, 0, null, null, null, null);

            var result = await _validator.ValidateAsync(request);

            Assert.False(result.IsValid);
            Assert.Equal(2, result.Errors.Count);

            Assert.Contains(result.Errors,
                x => x.PropertyName == nameof(Request.PageNumber)
                     && x.ErrorMessage == "Page number must be greater than 0.");

            Assert.Contains(result.Errors,
                x => x.PropertyName == nameof(Request.PageSize)
                     && x.ErrorMessage == "'Page Size' must be greater than '0'.");
        }

        [Fact]
        public async Task ViewUserListValidator_NullPaging_ShouldPassValidation()
        {
            var request = new Request(null, null, null, null, null, null);

            var result = await _validator.ValidateAsync(request);

            Assert.True(result.IsValid);
            Assert.Empty(result.Errors);
        }

        [Fact]
        public async Task ViewUserListValidator_ValidRequest_ShouldPassValidation()
        {
            var request = new Request(1, 20, "staff", "Admin", "staffCode", false);

            var result = await _validator.ValidateAsync(request);

            Assert.True(result.IsValid);
            Assert.Empty(result.Errors);
        }
    }
}
