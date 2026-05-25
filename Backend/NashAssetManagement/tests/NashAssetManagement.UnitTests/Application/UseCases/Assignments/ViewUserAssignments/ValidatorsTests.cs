using FluentValidation.TestHelper;
using NashAssetManagement.Application.UseCases.Assignments.ViewUserAssignments;
using Xunit;

namespace NashAssetManagement.UnitTests.Application.UseCases.Assignments.ViewUserAssignments
{
    public class ValidatorsTests
    {
        private readonly Validators _validator;

        public ValidatorsTests()
        {
            _validator = new Validators();
        }

        [Fact]
        public void TestValidate_ShouldHaveValidationErrorForPageSize_WhenPageSizeIsTooLarge()
        {
            // Arrange
            var request = new Request(
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
    }
}
