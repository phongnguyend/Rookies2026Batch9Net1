using FluentValidation;

namespace NashAssetManagement.Application.UseCases.Assets;

public class GetAssetDetailValidator : AbstractValidator<GetAssetDetailRequest>
{
    public GetAssetDetailValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty().WithMessage("Asset id is required.");
    }
}
