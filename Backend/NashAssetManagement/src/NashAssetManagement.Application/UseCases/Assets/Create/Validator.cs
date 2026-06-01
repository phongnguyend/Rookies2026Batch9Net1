using FluentValidation;
using NashAssetManagement.Domain.Enums;

namespace NashAssetManagement.Application.UseCases.Assets.Create;

public class CreateAssetValidator : AbstractValidator<CreateAssetRequest>
{
    public CreateAssetValidator()
    {
        RuleFor(x => x.AssetName)
            .NotEmpty().WithMessage("Asset name is required.")
            .MaximumLength(100).WithMessage("Asset name must not exceed 100 characters.");

        RuleFor(x => x.Specification)
            .NotEmpty().WithMessage("Specification is required.")
            .MaximumLength(500).WithMessage("Specification must not exceed 500 characters.");

        RuleFor(x => x.InstalledDate)
            .Must(date => date.Date <= DateTime.UtcNow.Date)
            .WithMessage("Installed date cannot be in the future.");

        RuleFor(x => x.State)
            .Must(s => s == AssetState.Available || s == AssetState.NotAvailable)
            .WithMessage("State must be Available or NotAvailable.");
    }
}