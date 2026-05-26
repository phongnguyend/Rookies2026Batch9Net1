

using FluentValidation;
using NashAssetManagement.Application.Common.Validators;

namespace NashAssetManagement.Application.UseCases.Users.ViewList
{
    public class Validators : AbstractValidator<Request>
    {
        public Validators()
        {
            RuleFor(x => x.PageNumber)
                .MustBeValidPageNumber();
            RuleFor(x => x.PageSize)
                .MustBeValidPageSize();   
        }
    }
}