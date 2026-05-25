using FluentValidation;
using NashAssetManagement.Application.Abstractions.DataAccess;
using NashAssetManagement.Application.UseCases.Assets.Specification;
using NashAssetManagement.Domain.Entities.Core;
using NashAssetManagement.Domain.Enums;

namespace NashAssetManagement.Application.UseCases.Assets.ViewList;

public class GetAssetsValidator : AbstractValidator<GetAssetsRequest>
{
    private readonly IRepository<Category, Guid> _categoryRepository;

    public GetAssetsValidator(IRepository<Category, Guid> categoryRepository)
    {
        _categoryRepository = categoryRepository;

        RuleForEach(x => x.States)
            .Must(s => Enum.TryParse<AssetState>(s, out _))
            .WithMessage(s => $"State '{s}' is invalid. Must be one of: {string.Join(", ", Enum.GetNames<AssetState>())}.");

        RuleForEach(x => x.Categories)
            .MustAsync(CategoryExistsAsync)
            .WithMessage((request, categoryName) => $"Category '{categoryName}' does not exist.");

        RuleFor(x => x.PageNumber)
            .GreaterThanOrEqualTo(1)
            .WithMessage("PageNumber must be at least 1.");

        RuleFor(x => x.PageSize)
            .InclusiveBetween(1, 50)
            .WithMessage("PageSize must be between 1 and 50.");
    }

    private async Task<bool> CategoryExistsAsync(
        string categoryName,
        CancellationToken cancellationToken)
    {
        var spec = new CategoryByNameSpec(categoryName);
        return await _categoryRepository.AnyAsync(spec, cancellationToken);
    }
}