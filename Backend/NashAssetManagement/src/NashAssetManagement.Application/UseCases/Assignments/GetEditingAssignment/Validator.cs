using FluentValidation;
using NashAssetManagement.Application.Common.Validators;

namespace NashAssetManagement.Application.UseCases.Assignments.GetEditingAssignment
{
    public class Validator
        : AbstractValidator<Request>
    {
        public Validator()
        {
            RuleFor(x => x.AssignmentId)
                .MustBeValidGuid();
        }
    }
}
