using FluentValidation;
using NashAssetManagement.Application.Abstractions.DataAccess;
using NashAssetManagement.Application.UseCases.Assets.Specification;
using NashAssetManagement.Application.UseCases.Categories.Specification;
using NashAssetManagement.Domain.Entities.Core;
using NashAssetManagement.Domain.Enums;
namespace NashAssetManagement.Application.UseCases.Assets.ViewList;

public class GetAssetsValidator : AbstractValidator<GetAssetsRequest>
{
    private readonly IRepository<Category, Guid> _categoryRepository;
    private static readonly string[] AllowedSortBy = ["assetcode", "name", "category", "state"];
    private static readonly string[] AllowedSortDirection = ["asc", "desc"];
    public GetAssetsValidator(IRepository<Category, Guid> categoryRepository)
    {
        _categoryRepository = categoryRepository;

        RuleFor(x => x.SortBy)
            .Must(s => s is null || AllowedSortBy.Contains(s.ToLower()))
            .WithMessage($"SortBy must be one of: {string.Join(", ", AllowedSortBy)}.");

        RuleFor(x => x.Search)
            .MaximumLength(100)
            .WithMessage("Search must not exceed 100 characters.")
            .Must(search => search == null || search.Trim().Length > 0)
            .WithMessage("Search cannot contain only whitespace.");

        RuleFor(x => x.SortDirection)
            .Must(s => s is null || AllowedSortDirection.Contains(s.ToLower()))
            .WithMessage($"SortDirection must be one of: {string.Join(", ", AllowedSortDirection)}.");

        RuleFor(x => x.States)
            .Must(states => states == null || states
                .Split(",", StringSplitOptions.RemoveEmptyEntries)
                .All(s => Enum.TryParse<AssetState>(s, out _)))
            .WithMessage($"State is invalid. Must be one of: {string.Join(", ", Enum.GetNames<AssetState>())}.");

        RuleFor(x => x.Categories)
            .MustAsync(async (categories, cancellationToken) =>
            {
                if (categories == null) return true;
                var categoryList = categories.Split(",", StringSplitOptions.RemoveEmptyEntries);
                foreach (var category in categoryList)
                {
                    var exists = await CategoryExistsAsync(category, cancellationToken);
                    if (!exists) return false;
                }
                return true;
            })
            .WithMessage(x => $"One or more categories do not exist.");

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