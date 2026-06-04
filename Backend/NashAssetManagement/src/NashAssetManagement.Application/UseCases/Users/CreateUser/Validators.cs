using FluentValidation;
using NashAssetManagement.Domain.Enums;

namespace NashAssetManagement.Application.UseCases.Users.CreateUser
{
    public class Validators : AbstractValidator<Request>
    {
        public Validators()
        {
            RuleFor(x => x.FirstName)
                .NotEmpty()
                .WithMessage("First Name is required.")
                .MaximumLength(100)
                .WithMessage("First Name must not exceed 100 characters.")
                .Matches(@"^[a-zA-Z]+(?: [a-zA-Z]+)*$")
                .WithMessage("First Name only allows alphabetic characters and spaces.");

            RuleFor(x => x.LastName)
                .NotEmpty()
                .WithMessage("Last Name is required.")
                .MaximumLength(100)
                .WithMessage("Last Name must not exceed 100 characters.")
                .Matches(@"^[a-zA-Z]+(?: [a-zA-Z]+)*$")
                .WithMessage("Last Name only allows alphabetic characters and spaces.");

            // 18 <= DOB <= 90
            RuleFor(x => x.DayOfBirth)
                .NotEmpty()
                .WithMessage("Date of Birth is required.")
                .Must(dayOfBirth => dayOfBirth.Date <= DateTime.UtcNow.Date.AddYears(-18))
                .WithMessage("User is under 18. Please select a different date.")
                .Must(dayOfBirth => dayOfBirth.Date >= DateTime.UtcNow.Date.AddYears(-90))
                .WithMessage("User age must not exceed 90 years.");

            // 18 <= JD <= now + 1 month && not Sun and Sat
            RuleFor(x => x.JoinedDate)
                .NotEmpty()
                .WithMessage("Joined Date is required.")
                .Must((request, joinedDate) => joinedDate.Date > request.DayOfBirth.Date)
                .WithMessage("Joined date is not later than Date of Birth. Please select a different date")
                .Must(dayOfBirth => dayOfBirth.Date <= DateTime.UtcNow.Date.AddMonths(1))
                .WithMessage("Joined date must not exceed one month. Please select a different date")
                .Must(joinedDate =>
                    joinedDate.DayOfWeek != DayOfWeek.Saturday &&
                    joinedDate.DayOfWeek != DayOfWeek.Sunday)
                .WithMessage("Joined date must not in Saturday or Sunday. Please select a different date");

            RuleFor(x => x.Gender)
                .NotNull()
                .WithMessage("Gender is required.")
                .IsInEnum()
                .WithMessage("Gender must be valid. Must be: Male or Female");

            RuleFor(x => x.UserType)
                .NotNull()
                .WithMessage("Type is required.")
                .IsInEnum()
                .WithMessage("Type must be valid. Must be: Admin or Staff");
        }
    }
}