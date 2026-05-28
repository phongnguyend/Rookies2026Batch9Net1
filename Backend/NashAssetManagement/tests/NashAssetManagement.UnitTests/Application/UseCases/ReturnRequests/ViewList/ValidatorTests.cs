using FluentValidation.TestHelper;
using NashAssetManagement.Application.UseCases.ReturnRequests.ViewList;
using NashAssetManagement.Domain.Enums;
using Xunit;

namespace NashAssetManagement.UnitTests.Application.UseCases.ReturnRequests.ViewList
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
            var request = new Request(
                SearchTerm: new string('a', 101),
                States: null,
                ReturnedDate: null,
                SortBy: null,
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
            Assert.Equal(nameof(Request.SearchTerm), error.PropertyName);
            Assert.Equal("Search term must not exceed 100 characters.", error.ErrorMessage);
        }

        [Fact]
        public void TestValidate_ShouldHaveValidationErrorForPageSize_WhenPageSizeIsTooLarge()
        {
            // Arrange
            var request = new Request(
                SearchTerm: null,
                States: null,
                ReturnedDate: null,
                SortBy: null,
                SortDesc: false,
                PageSize: 101,
                PageNumber: 1
            );

            // Act
            var result = _validator.TestValidate(request);

            // Assert
            result.ShouldHaveValidationErrorFor(x => x.PageSize);
            Assert.Single(result.Errors);
            var error = result.Errors[0];
            Assert.Equal(nameof(Request.PageSize), error.PropertyName);
            Assert.Equal("The page size is invalid", error.ErrorMessage);
        }

        [Fact]
        public void TestValidate_ShouldHaveValidationErrorForPageSize_WhenPageSizeIs0()
        {
            // Arrange
            var request = new Request(
                SearchTerm: null,
                States: null,
                ReturnedDate: null,
                SortBy: null,
                SortDesc: false,
                PageSize: 0,
                PageNumber: 1
            );

            // Act
            var result = _validator.TestValidate(request);

            // Assert
            result.ShouldHaveValidationErrorFor(x => x.PageSize);
            Assert.Single(result.Errors);
            var error = result.Errors[0];
            Assert.Equal(nameof(Request.PageSize), error.PropertyName);
            Assert.Equal("'Page Size' must be greater than '0'.", error.ErrorMessage);
        }

        [Fact]
        public void TestValidate_ShouldHaveValidationErrorForPageNumber_WhenPageNumberIs0()
        {
            // Arrange
            var request = new Request(
                SearchTerm: null,
                States: null,
                ReturnedDate: null,
                SortBy: null,
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
            Assert.Equal(nameof(Request.PageNumber), error.PropertyName);
            Assert.Equal("The page number is invalid", error.ErrorMessage);
        }

        [Fact]
        public void TestValidate_ShouldHaveValidationErrorForPageNumber_WhenPageNumberIsNegative()
        {
            // Arrange
            var request = new Request(
                SearchTerm: null,
                States: null,
                ReturnedDate: null,
                SortBy: null,
                SortDesc: false,
                PageSize: 10,
                PageNumber: -1
            );

            // Act
            var result = _validator.TestValidate(request);

            // Assert
            result.ShouldHaveValidationErrorFor(x => x.PageNumber);
            Assert.Single(result.Errors);
            var error = result.Errors[0];
            Assert.Equal(nameof(Request.PageNumber), error.PropertyName);
            Assert.Equal("The page number is invalid", error.ErrorMessage);
        }

        [Fact]
        public void TestValidate_ShouldHaveValidationErrorForStates_WhenStateIsInvalid()
        {
            // Arrange
            var request = new Request(
                SearchTerm: null,
                States: [(ReturnRequestState)999],
                ReturnedDate: null,
                SortBy: null,
                SortDesc: false,
                PageSize: 10,
                PageNumber: 1
            );

            // Act
            var result = _validator.TestValidate(request);

            // Assert
            result.ShouldHaveValidationErrorFor("States[0]");
            Assert.Single(result.Errors);
            var error = result.Errors[0];
            Assert.Equal("States[0]", error.PropertyName);
            Assert.Equal("Invalid state value: 999.", error.ErrorMessage);
        }

        [Fact]
        public void TestValidate_ShouldHaveValidationErrorForStates_WhenStateIsOutOfRange()
        {
            // Arrange
            var request = new Request(
                SearchTerm: null,
                States: [(ReturnRequestState)999],
                ReturnedDate: null,
                SortBy: null,
                SortDesc: false,
                PageSize: 10,
                PageNumber: 1
            );

            // Act
            var result = _validator.TestValidate(request);

            // Assert
            result.ShouldHaveValidationErrorFor("States[0]");
            Assert.Single(result.Errors);
            var error = result.Errors[0];
            Assert.Equal("States[0]", error.PropertyName);
            Assert.Equal("Invalid state value: 999.", error.ErrorMessage);
        }

        [Fact]
        public void TestValidate_ShouldHaveValidationErrors_WhenMultipleFieldsAreInvalid()
        {
            // Arrange
            var request = new Request(
                SearchTerm: null,
                States: [(ReturnRequestState)999],
                ReturnedDate: null,
                SortBy: null,
                SortDesc: false,
                PageSize: 0,
                PageNumber: 0
            );

            // Act
            var result = _validator.TestValidate(request);

            // Assert
            result.ShouldHaveValidationErrorFor(x => x.PageNumber);
            result.ShouldHaveValidationErrorFor(x => x.PageSize);
            result.ShouldHaveValidationErrorFor("States[0]");
            Assert.Equal(3, result.Errors.Count);
            Assert.Contains(result.Errors,
                x => x.PropertyName == nameof(Request.PageNumber)
                     && x.ErrorMessage == "The page number is invalid");
            Assert.Contains(result.Errors,
                x => x.PropertyName == nameof(Request.PageSize)
                     && x.ErrorMessage == "'Page Size' must be greater than '0'.");
            Assert.Contains(result.Errors,
                x => x.PropertyName == "States[0]"
                     && x.ErrorMessage == "Invalid state value: 999.");
        }

        [Fact]
        public void TestValidate_ShouldNotHaveAnyValidationErrors_WhenRequestIsValid()
        {
            // Arrange
            var request = new Request(
                SearchTerm: "laptop",
                States:
                [
                    ReturnRequestState.WaitingForReturning,
                    ReturnRequestState.Completed
                ],
                ReturnedDate: "2026-05-26",
                SortBy: "assetCode",
                SortDesc: false,
                PageSize: 20,
                PageNumber: 1
            );

            // Act
            var result = _validator.TestValidate(request);

            // Assert
            result.ShouldNotHaveAnyValidationErrors();
        }

        [Fact]
        public void TestValidate_ShouldNotHaveAnyValidationErrors_WhenValuesAreNulls()
        {
            // Arrange
            var request = new Request(
                SearchTerm: null,
                States: null,
                ReturnedDate: null,
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
