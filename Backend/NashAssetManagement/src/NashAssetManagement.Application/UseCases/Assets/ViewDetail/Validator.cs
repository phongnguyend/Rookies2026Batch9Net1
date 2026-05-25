using FluentValidation;

namespace NashAssetManagement.Application.UseCases.Assets.ViewDetail;

public class GetAssetDetailValidator : AbstractValidator<GetAssetDetailRequest>
{
    public GetAssetDetailValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty().WithMessage("Asset id is required.");
    }
}
