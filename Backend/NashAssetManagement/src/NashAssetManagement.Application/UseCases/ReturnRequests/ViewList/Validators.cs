using FluentValidation;
using NashAssetManagement.Application.Common.Validators;
using NashAssetManagement.Domain.Enums;

namespace NashAssetManagement.Application.UseCases.ReturnRequests.ViewList
{
    public class Validators : AbstractValidator<Request>
    {
        public Validators()
        {
            RuleFor(x => x.PageNumber)
                .MustBeValidPageNumber();
            RuleFor(x => x.PageSize)
                .MustBeValidPageSize();
            RuleForEach(x => x.States)
                .Must(state =>
                    !string.IsNullOrWhiteSpace(state) &&
                    Enum.TryParse<ReturnRequestState>(state.Trim(), true, out _))
                .WithMessage((_, state) => $"Invalid state value: {state}.");
        }
    }
}
