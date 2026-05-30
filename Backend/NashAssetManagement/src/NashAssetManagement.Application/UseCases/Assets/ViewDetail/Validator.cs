using FluentValidation;
using NashAssetManagement.Application.Common.Validators;

namespace NashAssetManagement.Application.UseCases.Assets.ViewDetail;

public class GetAssetDetailValidator : AbstractValidator<GetAssetDetailRequest>
{
    public GetAssetDetailValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty().MustBeValidGuid()
            .WithMessage("Asset id is required");
    }
}
