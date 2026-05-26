using FluentValidation;
using NashAssetManagement.Application.Common.Validators;

namespace NashAssetManagement.Application.UseCases.Users.ViewDetail
{
    public class Validators : AbstractValidator<Request>
    {
        public Validators()
        {
            RuleFor(x => x.UserId)
                .MustBeValidGuid();
        }
    }
}