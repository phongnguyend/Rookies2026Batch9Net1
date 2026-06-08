using FluentValidation;
using NashAssetManagement.Application.Common.Validators;

namespace NashAssetManagement.Application.UseCases.Users.EditUser
{
    public class Validators : AbstractValidator<Request>
    {
        public Validators()
        {
            RuleFor(x => x.UserId)
                .MustBeValidGuid();

            RuleFor(x => x.ConcurrencyStamp)
                .NotEmpty()
                .WithMessage("Concurrency stamp is required.");

            RuleFor(x => x.DateOfBirth)
                .Cascade(CascadeMode.Stop)
                .NotEmpty()
                .WithMessage("Date of Birth is required")
                .Must(dob => dob.Date <= DateTime.Today)
                .WithMessage("Date of birth cannot be in the future. Please select a different date")
                .Must(dob => dob.Date <= DateTime.Today.AddYears(-18))
                .WithMessage("User is under 18. Please select a different date")
                .Must(dob => dob.Date >= DateTime.Today.AddYears(-90))
                .WithMessage("User age must not exceed 90 years.");

            RuleFor(x => x.JoinedDate)
                .Cascade(CascadeMode.Stop)
                .NotEmpty()
                .WithMessage("Joined Date is required")
                .Must((request, joinedDate) => joinedDate.Date > request.DateOfBirth.Date)
                .WithMessage("Joined date is not later than Date of Birth. Please select a different date")
                .Must((request, joinedDate) => joinedDate.Date >= request.DateOfBirth.Date.AddYears(18))
                .WithMessage("User must be at least 18 years old at the Joined Date")
                .Must(joinedDate => 
                    joinedDate.DayOfWeek != DayOfWeek.Saturday &&
                    joinedDate.DayOfWeek != DayOfWeek.Sunday)
                .WithMessage("Joined date is Saturday or Sunday. Please select a different date");

            RuleFor(x => x.Gender)
                .IsInEnum()
                .WithMessage("Invalid gender. Please select a valid gender");

            RuleFor(x => x.Type)
                .IsInEnum()
                .WithMessage("Invalid type. Please select a valid type");
        }   
    }
}
