using FluentValidation;
using NashAssetManagement.Application.Common.Validators;

namespace NashAssetManagement.Application.UseCases.ReturnRequests.AdminCancelReturnRequest
{
    public class Validator
        : AbstractValidator<Request>
    {
        public Validator()
        {
            RuleFor(x => x.ReturnRequestId)
                .MustBeValidGuid();
        }
    }
}
