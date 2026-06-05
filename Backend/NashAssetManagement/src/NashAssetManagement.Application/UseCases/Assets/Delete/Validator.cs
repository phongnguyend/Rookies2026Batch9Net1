using FluentValidation;
using NashAssetManagement.Application.Common.Validators;

namespace NashAssetManagement.Application.UseCases.Assets.Delete;

public class DeleteAssetValidator : AbstractValidator<DeleteAssetRequest>
{
    public DeleteAssetValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty().WithMessage("Asset id is required")
            .MustBeValidGuid();
    }
}
