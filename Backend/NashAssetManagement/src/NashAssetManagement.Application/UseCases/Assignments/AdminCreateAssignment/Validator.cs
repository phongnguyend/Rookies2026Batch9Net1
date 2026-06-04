using FluentValidation;
using NashAssetManagement.Application.Abstractions.DateTimes;
using NashAssetManagement.Application.Common.Validators;

namespace NashAssetManagement.Application.UseCases.Assignments.AdminCreateAssignment
{
    public class Validator : AbstractValidator<Request>
    {
        public Validator(IDateTimeProvider dateTimeProvider)
        {
            RuleFor(x => x.UserId)
                .MustBeValidGuid();

            RuleFor(x => x.AssetId)
                .MustBeValidGuid();

            RuleFor(x => x.Note)
                .MaximumLength(1000)
                .WithMessage("Note cannot exceed 1000 characters.");

            RuleFor(x => x.AssignedDate)
                .NotEmpty()
                .WithMessage("Assigned date is required.")
                .GreaterThanOrEqualTo(_ => dateTimeProvider.UtcNow.Date)
                .WithMessage("Assigned date must be current date or in the future.");
        }
    }
}
