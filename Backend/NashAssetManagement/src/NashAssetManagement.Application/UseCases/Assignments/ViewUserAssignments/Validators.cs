using FluentValidation;
using NashAssetManagement.Application.Common.Validators;

namespace NashAssetManagement.Application.UseCases.Assignments.ViewUserAssignments
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
