using FluentValidation;
using NashAssetManagement.Application.Abstractions.DataAccess;
using NashAssetManagement.Application.UseCases.Assets.Specification;
using NashAssetManagement.Domain.Entities.Core;
using NashAssetManagement.Domain.Enums;

namespace NashAssetManagement.Application.UseCases.Assets;

public class GetAssetsValidator : AbstractValidator<GetAssetsRequest>
{
    private readonly IRepository<Category, Guid> _categoryRepository;

    public GetAssetsValidator(IRepository<Category, Guid> categoryRepository)
    {
        _categoryRepository = categoryRepository;

        RuleFor(x => x.State)
            .IsInEnum()
            .When(x => x.State is not null)
            .WithMessage($"State must be one of: {string.Join(", ", Enum.GetNames<AssetState>())}.");

        RuleFor(x => x.Category)
            .MustAsync(CategoryExistsAsync)
            .When(x => x.Category is not null)
            .WithMessage(x => $"Category '{x.Category}' does not exist.");

        RuleFor(x => x.PageNumber)
            .GreaterThanOrEqualTo(1)
            .WithMessage("PageNumber must be at least 1.");

        RuleFor(x => x.PageSize)
            .InclusiveBetween(1, 50)
            .WithMessage("PageSize must be between 1 and 50.");
    }

    private async Task<bool> CategoryExistsAsync(
        string? categoryName,
        CancellationToken cancellationToken)
    {
        var spec = new CategoryByNameSpec(categoryName!);
        return await _categoryRepository.AnyAsync(spec, cancellationToken);
    }
}

