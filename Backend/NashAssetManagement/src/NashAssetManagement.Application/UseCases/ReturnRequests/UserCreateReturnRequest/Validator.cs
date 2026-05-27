using FluentValidation;
using NashAssetManagement.Application.Common.Validators;

namespace NashAssetManagement.Application.UseCases.ReturnRequests.UserCreateReturnRequest
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
