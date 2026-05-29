using FluentValidation;
using NashAssetManagement.Application.Common.Validators;

namespace NashAssetManagement.Application.UseCases.Users.ViewUsers
{
    public class Validators : AbstractValidator<Request>
    {
        private const int MaxSearchTermLength = 100;

        public Validators()
        {
            RuleFor(x => x.SearchTerm)
                .MaximumLength(MaxSearchTermLength)
                .WithMessage($"Search term must not exceed {MaxSearchTermLength} characters.");

            RuleFor(x => x.PageNumber)
                .MustBeValidPageNumber();

            RuleFor(x => x.PageSize)
                .MustBeValidPageSize();
        }
    }
}
