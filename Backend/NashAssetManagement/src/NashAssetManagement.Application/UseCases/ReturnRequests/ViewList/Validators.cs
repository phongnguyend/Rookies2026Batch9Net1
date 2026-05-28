using FluentValidation;
using NashAssetManagement.Application.Common.Validators;

namespace NashAssetManagement.Application.UseCases.ReturnRequests.ViewList
{
    public class Validators : AbstractValidator<Request>
    {
        public Validators()
        {
            RuleFor(x => x.SearchTerm)
                .MaximumLength(100)
                .WithMessage("Search term must not exceed 100 characters.");
            RuleFor(x => x.PageNumber)
                .MustBeValidPageNumber().WithMessage("The page number is invalid");
            RuleFor(x => x.PageSize)
                .MustBeValidPageSize().WithMessage("The page size is invalid");
            RuleForEach(x => x.States)
                .IsInEnum()
                .WithMessage((_, state) => $"Invalid state value: {state}.");
        }
    }
}
