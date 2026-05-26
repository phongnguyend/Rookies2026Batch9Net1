using NashAssetManagement.Application.UseCases.ReturnRequests.ViewList;
using NashAssetManagement.Domain.Enums;
using Xunit;

namespace NashAssetManagement.UnitTests.Application.UseCases.ReturnRequests.ViewList
{
    public class ValidatorTests
    {
        private readonly Validators _validator = new();

        [Fact]
        public async Task ViewReturnRequestListValidator_PageNumberIsZero_ShouldReturnErrors()
        {
            // Arrange
            var request = new Request(null, null, null, null, null, 10, 0);

            // Act
            var result = await _validator.ValidateAsync(request);

            // Assert
            Assert.False(result.IsValid);
            Assert.Contains(result.Errors,
                x => x.PropertyName == nameof(Request.PageNumber)
                     && x.ErrorMessage == "Page number must be greater than 0.");
        }

        [Fact]
        public async Task ViewReturnRequestListValidator_PageNumberIsNegative_ShouldReturnErrors()
        {
            // Arrange
            var request = new Request(null, null, null, null, null, 10, -1);

            // Act
            var result = await _validator.ValidateAsync(request);

            // Assert
            Assert.False(result.IsValid);
            Assert.Contains(result.Errors,
                x => x.PropertyName == nameof(Request.PageNumber)
                     && x.ErrorMessage == "Page number must be greater than 0.");
        }

        [Fact]
        public async Task ViewReturnRequestListValidator_PageSizeIsZero_ShouldReturnErrors()
        {
            // Arrange
            var request = new Request(null, null, null, null, null, 0, 1);

            // Act
            var result = await _validator.ValidateAsync(request);

            // Assert
            Assert.False(result.IsValid);
            Assert.Contains(result.Errors,
                x => x.PropertyName == nameof(Request.PageSize)
                     && x.ErrorMessage == "'Page Size' must be greater than '0'.");
        }

        [Fact]
        public async Task ViewReturnRequestListValidator_PageSizeIsGreaterThanMax_ShouldReturnErrors()
        {
            // Arrange
            var request = new Request(null, null, null, null, null, 101, 1);

            // Act
            var result = await _validator.ValidateAsync(request);

            // Assert
            Assert.False(result.IsValid);
            Assert.Contains(result.Errors,
                x => x.PropertyName == nameof(Request.PageSize)
                     && x.ErrorMessage == "Page size must be between 0 and 100.");
        }

        [Fact]
        public async Task ViewReturnRequestListValidator_StateIsInvalid_ShouldReturnErrors()
        {
            // Arrange
            var request = new Request(null, ["InvalidState"], null, null, null, 10, 1);

            // Act
            var result = await _validator.ValidateAsync(request);

            // Assert
            Assert.False(result.IsValid);
            Assert.Contains(result.Errors,
                x => x.PropertyName.StartsWith(nameof(Request.States))
                     && x.ErrorMessage == "Invalid state value: InvalidState.");
        }

        [Fact]
        public async Task ViewReturnRequestListValidator_StateIsWhitespace_ShouldReturnErrors()
        {
            // Arrange
            var request = new Request(null, ["   "], null, null, null, 10, 1);

            // Act
            var result = await _validator.ValidateAsync(request);

            // Assert
            Assert.False(result.IsValid);
            Assert.Contains(result.Errors,
                x => x.PropertyName.StartsWith(nameof(Request.States))
                     && x.ErrorMessage == "Invalid state value:    .");
        }

        [Fact]
        public async Task ViewReturnRequestListValidator_MultipleInvalidFields_ShouldReturnErrors()
        {
            // Arrange
            var request = new Request(null, ["InvalidState"], null, null, null, 0, 0);

            // Act
            var result = await _validator.ValidateAsync(request);

            // Assert
            Assert.False(result.IsValid);
            Assert.Equal(3, result.Errors.Count);

            Assert.Contains(result.Errors,
                x => x.PropertyName == nameof(Request.PageNumber)
                     && x.ErrorMessage == "Page number must be greater than 0.");

            Assert.Contains(result.Errors,
                x => x.PropertyName == nameof(Request.PageSize)
                     && x.ErrorMessage == "'Page Size' must be greater than '0'.");

            Assert.Contains(result.Errors,
                x => x.PropertyName.StartsWith(nameof(Request.States))
                     && x.ErrorMessage == "Invalid state value: InvalidState.");
        }

        [Fact]
        public async Task ViewReturnRequestListValidator_NullPagingAndNullStates_ShouldPassValidation()
        {
            // Arrange
            var request = new Request(null, null, null, null, null, null, null);

            // Act
            var result = await _validator.ValidateAsync(request);

            // Assert
            Assert.True(result.IsValid);
            Assert.Empty(result.Errors);
        }

        [Fact]
        public async Task ViewReturnRequestListValidator_ValidRequest_ShouldPassValidation()
        {
            // Arrange
            var request = new Request(
                SearchTerm: "laptop",
                States:
                [
                    ReturnRequestState.WaitingForReturning.ToString(),
                    ReturnRequestState.Completed.ToString()
                ],
                ReturnedDate: "2026-05-26",
                SortBy: "assetCode",
                SortDesc: false,
                PageSize: 20,
                PageNumber: 1);

            // Act
            var result = await _validator.ValidateAsync(request);

            // Assert
            Assert.True(result.IsValid);
            Assert.Empty(result.Errors);
        }
    }
}
