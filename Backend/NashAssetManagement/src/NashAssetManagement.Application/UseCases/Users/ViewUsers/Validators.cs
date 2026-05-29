using FluentValidation;

namespace NashAssetManagement.Application.UseCases.Users.ViewUsers
{
    public class Validators : AbstractValidator<Request>
    {
        private const int MaxSearchTermLength = 100;
        private const int MaxPageSize = 10;

        public Validators()
        {
            RuleFor(x => x.SearchTerm)
                .MaximumLength(MaxSearchTermLength)
                .WithMessage($"Search term must not exceed {MaxSearchTermLength} characters.");

            RuleFor(x => x.PageNumber)
                .GreaterThan(0)
                .WithMessage("Page number must be greater than 0.");

            RuleFor(x => x.PageSize)
                .GreaterThan(0)
                .WithMessage("Page size must be greater than 0.")
                .LessThanOrEqualTo(MaxPageSize)
                .WithMessage($"Page size must not exceed {MaxPageSize}.");
        }
    }
}
