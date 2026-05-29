using FluentValidation.TestHelper;
using NashAssetManagement.Application.UseCases.Users.ViewUsers;
using Xunit;

namespace NashAssetManagement.UnitTests.Application.UseCases.Users.ViewUsers
{
    public class ValidatorTests
    {
        private readonly Validators _validator;

        public ValidatorTests()
        {
            _validator = new Validators();
        }

        [Fact]
        public void TestValidate_ShouldHaveValidationErrorForSearchTerm_WhenSearchTermIsTooLong()
        {
            // Arrange
            var request = new Request(1, 10, new string('a', 101), null, null, null);

            // Act
            var result = _validator.TestValidate(request);

            // Assert
            result.ShouldHaveValidationErrorFor(x => x.SearchTerm);
            Assert.Single(result.Errors);
            var error = result.Errors[0];
            Assert.Equal(nameof(Request.SearchTerm), error.PropertyName);
            Assert.Equal("Search term must not exceed 100 characters.", error.ErrorMessage);
        }

        [Fact]
        public void TestValidate_ShouldHaveValidationErrorForPageNumber_WhenPageNumberIs0()
        {
            // Arrange
            var request = new Request(0, 10, null, null, null, null);

            // Act
            var result = _validator.TestValidate(request);

            // Assert
            result.ShouldHaveValidationErrorFor(x => x.PageNumber);
            Assert.Single(result.Errors);
            var error = result.Errors[0];
            Assert.Equal(nameof(Request.PageNumber), error.PropertyName);
            Assert.Equal("Page number must be greater than 0.", error.ErrorMessage);
        }

        [Fact]
        public void TestValidate_ShouldHaveValidationErrorForPageNumber_WhenPageNumberIsNegative()
        {
            // Arrange
            var request = new Request(-1, 10, null, null, null, null);

            // Act
            var result = _validator.TestValidate(request);

            // Assert
            result.ShouldHaveValidationErrorFor(x => x.PageNumber);
            Assert.Single(result.Errors);
            var error = result.Errors[0];
            Assert.Equal(nameof(Request.PageNumber), error.PropertyName);
            Assert.Equal("Page number must be greater than 0.", error.ErrorMessage);
        }

        [Fact]
        public void TestValidate_ShouldHaveValidationErrorForPageSize_WhenPageSizeIs0()
        {
            // Arrange
            var request = new Request(1, 0, null, null, null, null);

            // Act
            var result = _validator.TestValidate(request);

            // Assert
            result.ShouldHaveValidationErrorFor(x => x.PageSize);
            Assert.Single(result.Errors);
            var error = result.Errors[0];
            Assert.Equal(nameof(Request.PageSize), error.PropertyName);
            Assert.Equal("Page size must be greater than 0.", error.ErrorMessage);
        }

        [Fact]
        public void TestValidate_ShouldHaveValidationErrorForPageSize_WhenPageSizeIsTooLarge()
        {
            // Arrange
            var request = new Request(1, 11, null, null, null, null);

            // Act
            var result = _validator.TestValidate(request);

            // Assert
            result.ShouldHaveValidationErrorFor(x => x.PageSize);
            Assert.Single(result.Errors);
            var error = result.Errors[0];
            Assert.Equal(nameof(Request.PageSize), error.PropertyName);
            Assert.Equal("Page size must not exceed 10.", error.ErrorMessage);
        }

        [Fact]
        public void TestValidate_ShouldHaveValidationErrors_WhenPageNumberAndPageSizeAreInvalid()
        {
            // Arrange
            var request = new Request(0, 0, null, null, null, null);

            // Act
            var result = _validator.TestValidate(request);

            // Assert
            result.ShouldHaveValidationErrorFor(x => x.PageNumber);
            result.ShouldHaveValidationErrorFor(x => x.PageSize);
            Assert.Equal(2, result.Errors.Count);
            Assert.Contains(result.Errors,
                x => x.PropertyName == nameof(Request.PageNumber)
                     && x.ErrorMessage == "Page number must be greater than 0.");
            Assert.Contains(result.Errors,
                x => x.PropertyName == nameof(Request.PageSize)
                     && x.ErrorMessage == "Page size must be greater than 0.");
        }

        [Fact]
        public void TestValidate_ShouldNotHaveAnyValidationErrors_WhenValuesAreNulls()
        {
            // Arrange
            var request = new Request(null, null, null, null, null, null);

            // Act
            var result = _validator.TestValidate(request);

            // Assert
            result.ShouldNotHaveAnyValidationErrors();
        }

        [Fact]
        public void TestValidate_ShouldNotHaveAnyValidationErrors_WhenRequestIsValid()
        {
            // Arrange
            var request = new Request(1, 10, "staff", "Admin", "staffCode", false);

            // Act
            var result = _validator.TestValidate(request);

            // Assert
            result.ShouldNotHaveAnyValidationErrors();
        }
    }
}
