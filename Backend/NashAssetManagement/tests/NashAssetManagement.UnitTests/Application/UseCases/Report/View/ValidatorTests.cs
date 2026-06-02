using NashAssetManagement.Application.UseCases.Report.View;
using Xunit;

namespace NashAssetManagement.UnitTests.Application.UseCases.Report.View
{
    public class ValidatorTests
    {
        private readonly Validators _validator = new();

        [Fact]
        [Trait("UT", "ViewReport")]
        public async Task Validate_RequestIsValid_Pass()
        {
            // Arrange
            var request = new Request(10, 1, SortDirection.Asc, SortBy.Category);

            // Act
            var result = await _validator.ValidateAsync(request);

            // Assert
            Assert.True(result.IsValid);
        }

        [Fact]
        [Trait("UT", "ViewReport")]
        public async Task Validate_PageNumberIsZero_ReturnError()
        {
            // Arrange
            var request = new Request(10, 0, SortDirection.Asc, SortBy.Category);

            // Act
            var result = await _validator.ValidateAsync(request);

            // Assert
            Assert.False(result.IsValid);
            Assert.Contains(result.Errors, x => x.PropertyName == "PageNumber");
        }

        [Fact]
        [Trait("UT", "ViewReport")]
        public async Task Validate_PageSizeExceedsLimit_ReturnError()
        {
            // Arrange
            var request = new Request(100, 1, SortDirection.Asc, SortBy.Category);

            // Act
            var result = await _validator.ValidateAsync(request);

            // Assert
            Assert.False(result.IsValid);
            Assert.Contains(result.Errors, x => x.PropertyName == "PageSize");
        }

        [Fact]
        [Trait("UT", "ViewReport")]
        public async Task Validate_SortByIsInvalid_ReturnError()
        {
            // Arrange
            var request = new Request(10, 1, SortDirection.Asc, (SortBy)999);

            // Act
            var result = await _validator.ValidateAsync(request);

            // Assert
            Assert.False(result.IsValid);
            Assert.Contains(result.Errors, x => x.PropertyName == "SortBy");
            Assert.Contains(result.Errors, x => x.ErrorMessage == Validators.SortByMustBeValid);
        }

        [Fact]
        [Trait("UT", "ViewReport")]
        public async Task Validate_SortDirectionIsInvalid_ReturnError()
        {
            // Arrange
            var request = new Request(10, 1, (SortDirection)999, SortBy.Category);

            // Act
            var result = await _validator.ValidateAsync(request);

            // Assert
            Assert.False(result.IsValid);
            Assert.Contains(result.Errors, x => x.PropertyName == "SortDirection");
            Assert.Contains(result.Errors, x => x.ErrorMessage == Validators.SortDirectionMustBeValid);
        }
    }
}
