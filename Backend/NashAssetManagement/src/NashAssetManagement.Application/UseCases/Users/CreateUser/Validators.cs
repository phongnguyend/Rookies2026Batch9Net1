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
                .Matches(@"^[a-zA-Z\s]+$")
                .WithMessage("First Name only allows alphabetic characters and spaces.");

            RuleFor(x => x.LastName)
                .NotEmpty()
                .WithMessage("Last Name is required.")
                .MaximumLength(100)
                .WithMessage("Last Name must not exceed 100 characters.")
                .Matches(@"^[a-zA-Z\s]+$")
                .WithMessage("Last Name only allows alphabetic characters and spaces.");

            // 18 <= DOB <= 90
            RuleFor(x => x.DayOfBirth)
                .NotEmpty()
                .WithMessage("Date of Birth is required.")
                .Must(dayOfBirth => dayOfBirth.Date <= DateTime.UtcNow.Date.AddYears(-18))
                .WithMessage("User is under 18. Please select a different date.")
                .Must(dayOfBirth => dayOfBirth.Date >= DateTime.UtcNow.Date.AddYears(-90))
                .WithMessage("User age must not exceed 90 years.");

            // 18 <= JD <= now
            RuleFor(x => x.JoinedDate)
                .NotEmpty()
                .WithMessage("Joined Date is required.")
                .Must((request, joinedDate) => joinedDate.Date > request.DayOfBirth.Date)
                .WithMessage("Joined Date must be later than Date of Birth.");

            RuleFor(x => x.Gender)
                .IsInEnum()
                .WithMessage("Gender is required.");

            RuleFor(x => x.UserType)
                .IsInEnum()
                .WithMessage("Type is required.");
        }
    }
}