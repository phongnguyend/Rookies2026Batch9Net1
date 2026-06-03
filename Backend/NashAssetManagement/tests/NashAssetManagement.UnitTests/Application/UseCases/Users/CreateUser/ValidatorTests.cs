using FluentValidation.TestHelper;
using NashAssetManagement.Application.UseCases.Users.CreateUser;
using NashAssetManagement.Domain.Enums;
using Xunit;
// MethodName_StateUnderTest_ExpectedBehavior
namespace NashAssetManagement.UnitTests.Application.UseCases.Users.CreateUser
{
    public class ValidatorTests
    {
        private readonly Validators _validator;

        public ValidatorTests()
        {
            _validator = new Validators();
        }

        private static Request ValidRequest()
        {
            return new Request(
                FirstName: "Binh",
                LastName: "Nguyen Van",
                DayOfBirth: DateTime.UtcNow.Date.AddYears(-20),
                JoinedDate: GetNextWorkingDay(DateTime.UtcNow.Date),
                Gender: Gender.Male,
                UserType: UserType.Staff
            );
        }

        private static DateTime GetNextWorkingDay(DateTime date)
        {
            var nextDate = date;

            while (nextDate.DayOfWeek is DayOfWeek.Saturday or DayOfWeek.Sunday)
            {
                nextDate = nextDate.AddDays(1);
            }

            return nextDate;
        }

        [Fact]
        public void CreateUserValidator_FirstNameIsEmpty_ShouldReturnErrors()
        {
            var request = ValidRequest() with { FirstName = "" };

            var result = _validator.TestValidate(request);

            result.ShouldHaveValidationErrorFor(x => x.FirstName);
            Assert.Contains(result.Errors,
                x => x.PropertyName == nameof(Request.FirstName)
                     && x.ErrorMessage == "First Name is required.");
        }

        [Fact]
        public void CreateUserValidator_FirstNameExceeds100Characters_ShouldReturnErrors()
        {
            var request = ValidRequest() with { FirstName = new string('A', 101) };

            var result = _validator.TestValidate(request);

            result.ShouldHaveValidationErrorFor(x => x.FirstName);
            Assert.Contains(result.Errors,
                x => x.PropertyName == nameof(Request.FirstName)
                     && x.ErrorMessage == "First Name must not exceed 100 characters.");
        }

        [Fact]
        public void CreateUserValidator_FirstNameContainsInvalidCharacters_ShouldReturnErrors()
        {
            var request = ValidRequest() with { FirstName = "Binh123@" };

            var result = _validator.TestValidate(request);

            result.ShouldHaveValidationErrorFor(x => x.FirstName);
            Assert.Contains(result.Errors,
                x => x.PropertyName == nameof(Request.FirstName)
                     && x.ErrorMessage == "First Name only allows alphabetic characters and spaces.");
        }

        [Fact]
        public void CreateUserValidator_LastNameIsEmpty_ShouldReturnErrors()
        {
            var request = ValidRequest() with { LastName = "" };

            var result = _validator.TestValidate(request);

            result.ShouldHaveValidationErrorFor(x => x.LastName);
            Assert.Contains(result.Errors,
                x => x.PropertyName == nameof(Request.LastName)
                     && x.ErrorMessage == "Last Name is required.");
        }

        [Fact]
        public void CreateUserValidator_LastNameExceeds100Characters_ShouldReturnErrors()
        {
            var request = ValidRequest() with { LastName = new string('A', 101) };

            var result = _validator.TestValidate(request);

            result.ShouldHaveValidationErrorFor(x => x.LastName);
            Assert.Contains(result.Errors,
                x => x.PropertyName == nameof(Request.LastName)
                     && x.ErrorMessage == "Last Name must not exceed 100 characters.");
        }

        [Fact]
        public void CreateUserValidator_LastNameContainsInvalidCharacters_ShouldReturnErrors()
        {
            var request = ValidRequest() with { LastName = "Nguyen@123" };

            var result = _validator.TestValidate(request);

            result.ShouldHaveValidationErrorFor(x => x.LastName);
            Assert.Contains(result.Errors,
                x => x.PropertyName == nameof(Request.LastName)
                     && x.ErrorMessage == "Last Name only allows alphabetic characters and spaces.");
        }

        [Fact]
        public void CreateUserValidator_UserIsUnder18YearsOld_ShouldReturnErrors()
        {
            var request = ValidRequest() with
            {
                DayOfBirth = DateTime.UtcNow.Date.AddYears(-18).AddDays(1)
            };

            var result = _validator.TestValidate(request);

            result.ShouldHaveValidationErrorFor(x => x.DayOfBirth);
            Assert.Contains(result.Errors,
                x => x.PropertyName == nameof(Request.DayOfBirth)
                     && x.ErrorMessage == "User is under 18. Please select a different date.");
        }

        [Fact]
        public void CreateUserValidator_UserAgeExceeds90YearsOld_ShouldReturnErrors()
        {
            var request = ValidRequest() with
            {
                DayOfBirth = DateTime.UtcNow.Date.AddYears(-90).AddDays(-1)
            };

            var result = _validator.TestValidate(request);

            result.ShouldHaveValidationErrorFor(x => x.DayOfBirth);
            Assert.Contains(result.Errors,
                x => x.PropertyName == nameof(Request.DayOfBirth)
                     && x.ErrorMessage == "User age must not exceed 90 years.");
        }

        [Fact]
        public void CreateUserValidator_JoinedDateIsNotLaterThanDateOfBirth_ShouldReturnErrors()
        {
            var dob = DateTime.UtcNow.Date.AddYears(-20);

            var request = ValidRequest() with
            {
                DayOfBirth = dob,
                JoinedDate = dob
            };

            var result = _validator.TestValidate(request);

            result.ShouldHaveValidationErrorFor(x => x.JoinedDate);
            Assert.Contains(result.Errors,
                x => x.PropertyName == nameof(Request.JoinedDate)
                     && x.ErrorMessage == "Joined date is not later than Date of Birth. Please select a different date");
        }

        [Fact]
        public void CreateUserValidator_JoinedDateExceedsOneMonth_ShouldReturnErrors()
        {
            var request = ValidRequest() with
            {
                JoinedDate = DateTime.UtcNow.Date.AddMonths(1).AddDays(1)
            };

            var result = _validator.TestValidate(request);

            result.ShouldHaveValidationErrorFor(x => x.JoinedDate);

            Assert.Contains(result.Errors,
                x => x.PropertyName == nameof(Request.JoinedDate)
                    && x.ErrorMessage == "Joined date must not exceed one month. Please select a different date");
        }

        [Fact]
        public void CreateUserValidator_JoinedDateIsSaturday_ShouldReturnErrors()
        {
            var saturday = DateTime.UtcNow.Date;
            while (saturday.DayOfWeek != DayOfWeek.Saturday)
            {
                saturday = saturday.AddDays(1);
            }

            var request = ValidRequest() with { JoinedDate = saturday };

            var result = _validator.TestValidate(request);

            result.ShouldHaveValidationErrorFor(x => x.JoinedDate);
            Assert.Contains(result.Errors,
                x => x.PropertyName == nameof(Request.JoinedDate)
                     && x.ErrorMessage == "Joined date is Saturday or Sunday. Please select a different date");
        }

        [Fact]
        public void CreateUserValidator_JoinedDateIsSunday_ShouldReturnErrors()
        {
            var sunday = DateTime.UtcNow.Date;
            while (sunday.DayOfWeek != DayOfWeek.Sunday)
            {
                sunday = sunday.AddDays(1);
            }

            var request = ValidRequest() with { JoinedDate = sunday };

            var result = _validator.TestValidate(request);

            result.ShouldHaveValidationErrorFor(x => x.JoinedDate);
            Assert.Contains(result.Errors,
                x => x.PropertyName == nameof(Request.JoinedDate)
                     && x.ErrorMessage == "Joined date is Saturday or Sunday. Please select a different date");
        }

        [Fact]
        public void CreateUserValidator_GenderIsInvalid_ShouldReturnErrors()
        {
            var request = ValidRequest() with { Gender = (Gender)999 };

            var result = _validator.TestValidate(request);

            result.ShouldHaveValidationErrorFor(x => x.Gender);
            Assert.Contains(result.Errors,
                x => x.PropertyName == nameof(Request.Gender)
                     && x.ErrorMessage == "Gender must be valid. Must be: Male or Female");
        }

        [Fact]
        public void CreateUserValidator_UserTypeIsInvalid_ShouldReturnErrors()
        {
            var request = ValidRequest() with { UserType = (UserType)999 };

            var result = _validator.TestValidate(request);

            result.ShouldHaveValidationErrorFor(x => x.UserType);
            Assert.Contains(result.Errors,
                x => x.PropertyName == nameof(Request.UserType)
                     && x.ErrorMessage == "Type must be valid. Must be: Admin or Staff");
        }

        [Fact]
        public void CreateUserValidator_ValidRequest_ShouldPassValidation()
        {
            var request = ValidRequest();

            var result = _validator.TestValidate(request);

            result.ShouldNotHaveAnyValidationErrors();
        }
    }
}