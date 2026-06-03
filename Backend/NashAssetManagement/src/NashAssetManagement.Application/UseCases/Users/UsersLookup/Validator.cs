using FluentValidation;
using NashAssetManagement.Application.Common.Validators;

namespace NashAssetManagement.Application.UseCases.Users.UsersLookup
{
    public class Validator
        : AbstractValidator<Request>
    {
        public Validator()
        {
            RuleFor(x => x.SearchTerm)
                .MaximumLength(100)
                .WithMessage("Search term cannot exceed 100 characters.")
                .When(x => !string.IsNullOrWhiteSpace(x.SearchTerm));

            RuleFor(x => x.PageNumber)
               .MustBeValidPageNumber();

            RuleFor(x => x.PageSize)
                .MustBeValidPageSize();
        }
    }
}
