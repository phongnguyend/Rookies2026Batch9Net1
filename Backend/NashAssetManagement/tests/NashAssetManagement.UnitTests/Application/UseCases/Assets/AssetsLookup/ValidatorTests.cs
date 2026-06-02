using FluentValidation.TestHelper;
using NashAssetManagement.Application.UseCases.Assets.AssetsLookup;
using Xunit;

namespace NashAssetManagement.UnitTests.Application.UseCases.Assets.AssetsLookup
{
    public class ValidatorTests
    {
        readonly Validator _validator;

        public ValidatorTests()
        {
            _validator = new Validator();
        }

        [Fact]
        public void TestValidate_ShouldHaveValidationErrorForPageSize_WhenPageSizeIsTooLarge()
        {
            // Arrange
            var request = new Request(
                SearchTerm: "Search Term",
                SortBy: "Name",
                SortDesc: false,
                PageSize: 999,
                PageNumber: 1
            );

            // Act
            var result = _validator.TestValidate(request);

            // Assert
            result.ShouldHaveValidationErrorFor(x => x.PageSize);
            Assert.Single(result.Errors);
            var error = result.Errors[0];
            Assert.Equal("PageSize", error.PropertyName);
            Assert.Equal("Page size must be between 0 and 100.", error.ErrorMessage);
        }

        [Fact]
        public void TestValidate_ShouldNotHaveAnyValidationErrors_WhenRequestIsValid()
        {
            // Arrange
            var request = new Request(
                SearchTerm: "Search Term",
                SortBy: "Name",
                SortDesc: false,
                PageSize: 10,
                PageNumber: 1
            );

            // Act
            var result = _validator.TestValidate(request);

            // Assert
            result.ShouldNotHaveAnyValidationErrors();
        }

        [Fact]
        public void TestValidate_ShouldHaveValidationErrorForPageNumber_WhenPageNumberIs0()
        {
            // Arrange
            var request = new Request(
                SearchTerm: "Search Term",
                SortBy: "Name",
                SortDesc: false,
                PageSize: 10,
                PageNumber: 0
            );

            // Act
            var result = _validator.TestValidate(request);

            // Assert
            result.ShouldHaveValidationErrorFor(x => x.PageNumber);
            Assert.Single(result.Errors);
            var error = result.Errors[0];
            Assert.Equal("PageNumber", error.PropertyName);
            Assert.Equal("Page number must be greater than 0.", error.ErrorMessage);
        }

        [Fact]
        public void TestValidate_ShouldNotHaveAnyValidationErrors_WhenValuesAreNulls()
        {
            // Arrange
            var request = new Request(
                SearchTerm: null,
                SortBy: null,
                SortDesc: null,
                PageSize: null,
                PageNumber: null
            );

            // Act
            var result = _validator.TestValidate(request);

            // Assert
            result.ShouldNotHaveAnyValidationErrors();
        }

        [Fact]
        public void TestValidate_ShouldHaveValidationErrorForSearchTerm_WhenSearchTermIsTooLong()
        {
            // Arrange
            var invalidSearchTerm = "Lorem ipsum dolor sit amet, consectetuer adipiscing elit. Aenean commodo ligula eget dolor. Aenean massa.";
            var request = new Request(
                SearchTerm: invalidSearchTerm,
                SortBy: "Name",
                SortDesc: false,
                PageSize: 10,
                PageNumber: 1
            );

            // Act
            var result = _validator.TestValidate(request);

            // Assert
            result.ShouldHaveValidationErrorFor(x => x.SearchTerm);
            Assert.Single(result.Errors);
            var error = result.Errors[0];
            Assert.Equal("SearchTerm", error.PropertyName);
            Assert.Equal("Search term cannot exceed 100 characters.", error.ErrorMessage);
        }
    }
}
