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

            RuleFor(x => x.DateOfBirth)
                .NotEmpty()
                .Must(dob => dob.Date <= DateTime.Today.AddYears(-18))
                .WithMessage("User is under 18. Please select a different date");

            RuleFor(x => x.JoinedDate)
                .NotEmpty()
                .Must((request, joinedDate) => joinedDate.Date >= request.DateOfBirth.Date.AddYears(18))
                .WithMessage("Joined date is not later than Date of Birth. Please select a different date");

            RuleFor(x => x.JoinedDate)
                .NotEmpty()
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