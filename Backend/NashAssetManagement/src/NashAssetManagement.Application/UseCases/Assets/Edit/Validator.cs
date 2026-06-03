using FluentValidation;
using NashAssetManagement.Domain.Enums;
using System.Text.RegularExpressions;

namespace NashAssetManagement.Application.UseCases.Assets.Edit;

public class EditAssetValidator : AbstractValidator<EditAssetRequest>
{
    private const string AssetNamePattern = @"^[\p{L}\p{N}\s\-_\/]+$";

    public EditAssetValidator()
    {
        RuleFor(x => x.AssetName)
            .NotEmpty().WithMessage("Asset name is required.")
            .MaximumLength(100).WithMessage("Asset name must not exceed 100 characters.")
            .Matches(AssetNamePattern)
            .WithMessage("Asset name cannot contain special characters or emojis.");

        RuleFor(x => x.Specification)
            .NotEmpty().WithMessage("Specification is required.")
            .MaximumLength(500).WithMessage("Specification must not exceed 500 characters.")
            .Must(NotContainEmoji)
            .WithMessage("Specification cannot contain emojis.");

        RuleFor(x => x.State)
            .NotEqual(AssetState.Assigned)
            .WithMessage("State cannot be Assigned.");
    }

    private static bool NotContainEmoji(string? value)
    {
        if (string.IsNullOrWhiteSpace(value))
            return true;

        return !Regex.IsMatch(
            value,
            @"\p{So}|\p{Cs}",
            RegexOptions.CultureInvariant);
    }
}