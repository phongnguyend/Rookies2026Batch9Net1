using FluentValidation;
using NashAssetManagement.Application.Abstractions.DateTimes;
using NashAssetManagement.Application.Common.Validators;

namespace NashAssetManagement.Application.UseCases.Assignments.AdminEditAssignment
{
    public class Validator
        : AbstractValidator<Request>
    {
        public Validator(IDateTimeProvider dateTimeProvider)
        {
            RuleFor(x => x.AssignmentId)
                .MustBeValidGuid();

            RuleFor(x => x.Payload)
                .NotNull()
                .WithMessage("Assignment data is required for editing.");

            When(x => x.Payload != null, () =>
            {
                RuleFor(x => x.Payload!.UserId)
                    .MustBeValidGuid();

                RuleFor(x => x.Payload!.AssetId)
                    .MustBeValidGuid();

                RuleFor(x => x.Payload!.Note)
                    .MaximumLength(1000)
                    .WithMessage("Note cannot exceed 1000 characters.")
                    .When(x => !string.IsNullOrWhiteSpace(x.Payload!.Note));

                RuleFor(x => x.Payload!.AssignedDate)
                    .Must(date => date.Date >= dateTimeProvider.UtcNow.Date)
                    .WithMessage("Assigned date must be current date or in the future.");
            });
        }
    }
}
