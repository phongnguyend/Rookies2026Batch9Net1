using FluentValidation;
using NashAssetManagement.Application.Common.Validators;

namespace NashAssetManagement.Application.UseCases.Assignments.UserDecliningAssignment
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
