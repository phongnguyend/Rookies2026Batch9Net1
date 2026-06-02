using FluentValidation;
using NashAssetManagement.Application.Common.Validators;

namespace NashAssetManagement.Application.UseCases.Report.View
{
    public class Validators : AbstractValidator<Request>
    {
        public const string SortByMustBeValid = "SortBy must be one of the following: Category, Total, Assigned, Available, NotAvailable, WaitingForRecycling, Recycled.";
        public const string SortDirectionMustBeValid = "SortDirection must be either Asc or Desc.";

        public Validators()
        {
            RuleFor(x => x.PageNumber)
                .MustBeValidPageNumber();

            RuleFor(x => x.PageSize)
                .MustBeValidPageSize(10);

            RuleFor(x => x.SortBy)
                .IsInEnum().WithMessage(SortByMustBeValid)
                .When(x => x.SortBy.HasValue);

            RuleFor(x => x.SortDirection)
                .IsInEnum().WithMessage(SortDirectionMustBeValid)
                .When(x => x.SortDirection.HasValue);
        }
    }
}