using FluentValidation;

namespace NashAssetManagement.Application.UseCases.Report.Export
{
    public sealed class Validator : AbstractValidator<Request>
    {
        public const string SortByMustBeValid = "SortBy must be one of the following: Category, Total, Assigned, Available, NotAvailable, WaitingForRecycling, Recycled.";
        public const string SortDirectionMustBeValid = "SortDirection must be either Asc or Desc.";

        public Validator()
        {
            RuleFor(x => x.SortBy)
                .IsInEnum().WithMessage(SortByMustBeValid)
                .When(x => x.SortBy.HasValue);

            RuleFor(x => x.SortDirection)
                .IsInEnum().WithMessage(SortDirectionMustBeValid)
                .When(x => x.SortDirection.HasValue);
        }
    }
}