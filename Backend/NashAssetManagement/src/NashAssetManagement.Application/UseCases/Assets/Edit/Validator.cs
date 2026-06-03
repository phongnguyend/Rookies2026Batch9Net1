using FluentValidation;
using NashAssetManagement.Domain.Enums;

namespace NashAssetManagement.Application.UseCases.Assets.Edit;

public class EditAssetValidator : AbstractValidator<EditAssetRequest>
{
    private const string AssetNamePattern =
        @"^[\p{L}\p{N}\s]+$";

    private const string SpecificationPattern =
        @"^[\p{L}\p{N}\s,\/\-\|\(\)\+""']+$";

    public EditAssetValidator()
    {
        RuleFor(x => x.AssetName)
            .NotEmpty()
            .WithMessage("Asset name is required.")
            .MaximumLength(100)
            .WithMessage("Asset name must not exceed 100 characters.")
            .Matches(AssetNamePattern)
            .WithMessage(
                "Asset name can only contain letters, numbers and spaces."
            );

        RuleFor(x => x.Specification)
            .NotEmpty()
            .WithMessage("Specification is required.")
            .MaximumLength(500)
            .WithMessage("Specification must not exceed 500 characters.")
            .Matches(SpecificationPattern)
            .WithMessage(
                "Specification contains invalid characters."
            );

        RuleFor(x => x.State)
            .NotEqual(AssetState.Assigned)
            .WithMessage("State cannot be Assigned.");
    }
}