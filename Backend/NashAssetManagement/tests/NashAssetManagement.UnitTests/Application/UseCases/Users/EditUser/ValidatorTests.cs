using FluentValidation.TestHelper;
using NashAssetManagement.Application.UseCases.Users.EditUser;
using NashAssetManagement.Domain.Enums;
using Xunit;

namespace NashAssetManagement.UnitTests.Application.UseCases.Users.EditUser
{
    public class ValidatorTests
    {
        private readonly Validators _validator = new();

        [Fact]
        public void TestValidate_ShouldHaveValidationErrorForUserId_WhenUserIdIsNotGuid()
        {
            var request = CreateValidRequest(userId: "not-a-guid");

            var result = _validator.TestValidate(request);

            result.ShouldHaveValidationErrorFor(x => x.UserId);
            Assert.Contains(result.Errors,
                x => x.PropertyName == nameof(Request.UserId)
                     && x.ErrorMessage == "User Id must be a valid Guid/uuid.");
        }

        [Fact]
        public void TestValidate_ShouldHaveValidationErrorForConcurrencyStamp_WhenConcurrencyStampIsEmpty()
        {
            var request = CreateValidRequest(concurrencyStamp: "");

            var result = _validator.TestValidate(request);

            result.ShouldHaveValidationErrorFor(x => x.ConcurrencyStamp);
            Assert.Contains(result.Errors,
                x => x.PropertyName == nameof(Request.ConcurrencyStamp)
                     && x.ErrorMessage == "Concurrency stamp is required.");
        }

        [Fact]
        public void TestValidate_ShouldHaveValidationErrorForDateOfBirth_WhenDateOfBirthIsInFuture()
        {
            var request = CreateValidRequest(
                dateOfBirth: DateTime.Today.AddDays(1),
                joinedDate: DateTime.Today.AddYears(20));

            var result = _validator.TestValidate(request);

            result.ShouldHaveValidationErrorFor(x => x.DateOfBirth);
            Assert.Contains(result.Errors,
                x => x.PropertyName == nameof(Request.DateOfBirth)
                     && x.ErrorMessage == "Date of birth cannot be in the future. Please select a different date");
        }

        [Fact]
        public void TestValidate_ShouldHaveValidationErrorForDateOfBirth_WhenUserIsUnder18()
        {
            var request = CreateValidRequest(
                dateOfBirth: DateTime.Today.AddYears(-17),
                joinedDate: DateTime.Today.AddYears(1));

            var result = _validator.TestValidate(request);

            result.ShouldHaveValidationErrorFor(x => x.DateOfBirth);
            Assert.Contains(result.Errors,
                x => x.PropertyName == nameof(Request.DateOfBirth)
                     && x.ErrorMessage == "User is under 18. Please select a different date");
        }

        [Fact]
        public void TestValidate_ShouldHaveValidationErrorForJoinedDate_WhenJoinedDateIsNotLaterThanDateOfBirth()
        {
            var dateOfBirth = new DateTime(2000, 1, 3);
            var request = CreateValidRequest(dateOfBirth: dateOfBirth, joinedDate: dateOfBirth);

            var result = _validator.TestValidate(request);

            result.ShouldHaveValidationErrorFor(x => x.JoinedDate);
            Assert.Contains(result.Errors,
                x => x.PropertyName == nameof(Request.JoinedDate)
                     && x.ErrorMessage == "Joined date is not later than Date of Birth. Please select a different date");
            Assert.DoesNotContain(result.Errors,
                x => x.PropertyName == nameof(Request.JoinedDate)
                     && x.ErrorMessage == "User must be at least 18 years old on the joined date. Please select a different date");
        }

        [Fact]
        public void TestValidate_ShouldHaveValidationErrorForJoinedDate_WhenUserIsUnder18OnJoinedDate()
        {
            var dateOfBirth = new DateTime(2000, 1, 3);
            var request = CreateValidRequest(dateOfBirth: dateOfBirth, joinedDate: dateOfBirth.AddYears(18).AddDays(-1));

            var result = _validator.TestValidate(request);

            result.ShouldHaveValidationErrorFor(x => x.JoinedDate);
            Assert.Contains(result.Errors,
                x => x.PropertyName == nameof(Request.JoinedDate)
                     && x.ErrorMessage == "User must be at least 18 years old on the joined date. Please select a different date");
            Assert.DoesNotContain(result.Errors,
                x => x.PropertyName == nameof(Request.JoinedDate)
                     && x.ErrorMessage == "Joined date is not later than Date of Birth. Please select a different date");
        }

        [Fact]
        public void TestValidate_ShouldHaveValidationErrorForJoinedDate_WhenJoinedDateIsWeekend()
        {
            var request = CreateValidRequest(
                dateOfBirth: new DateTime(2000, 1, 3),
                joinedDate: new DateTime(2020, 1, 4));

            var result = _validator.TestValidate(request);

            result.ShouldHaveValidationErrorFor(x => x.JoinedDate);
            Assert.Contains(result.Errors,
                x => x.PropertyName == nameof(Request.JoinedDate)
                     && x.ErrorMessage == "Joined date is Saturday or Sunday. Please select a different date");
        }

        [Fact]
        public void TestValidate_ShouldHaveValidationErrorForGender_WhenGenderIsInvalid()
        {
            var request = CreateValidRequest(gender: (Gender)999);

            var result = _validator.TestValidate(request);

            result.ShouldHaveValidationErrorFor(x => x.Gender);
            Assert.Contains(result.Errors,
                x => x.PropertyName == nameof(Request.Gender)
                     && x.ErrorMessage == "Invalid gender. Please select a valid gender");
        }

        [Fact]
        public void TestValidate_ShouldHaveValidationErrorForType_WhenTypeIsInvalid()
        {
            var request = CreateValidRequest(type: (UserType)999);

            var result = _validator.TestValidate(request);

            result.ShouldHaveValidationErrorFor(x => x.Type);
            Assert.Contains(result.Errors,
                x => x.PropertyName == nameof(Request.Type)
                     && x.ErrorMessage == "Invalid type. Please select a valid type");
        }

        [Fact]
        public void TestValidate_ShouldNotHaveAnyValidationErrors_WhenRequestIsValid()
        {
            var request = CreateValidRequest();

            var result = _validator.TestValidate(request);

            result.ShouldNotHaveAnyValidationErrors();
        }

        private static Request CreateValidRequest(
            string? userId = "36c29308-4d9c-4e1b-9baf-a5dc11f26001",
            DateTime? dateOfBirth = null,
            Gender gender = Gender.Male,
            DateTime? joinedDate = null,
            UserType type = UserType.Staff,
            string? concurrencyStamp = "concurrency-stamp")
        {
            return new Request(
                userId,
                dateOfBirth ?? new DateTime(2000, 1, 3),
                gender,
                joinedDate ?? new DateTime(2020, 1, 6),
                type,
                concurrencyStamp);
        }
    }
}
