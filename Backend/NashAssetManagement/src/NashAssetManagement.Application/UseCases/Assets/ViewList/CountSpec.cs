using Ardalis.Specification;
using NashAssetManagement.Domain.Entities.Core;
using NashAssetManagement.Domain.Enums;

namespace NashAssetManagement.Application.UseCases.Assets.Specification;

public sealed class AssetCountSpec : Specification<Asset>
{
    public AssetCountSpec(string[]? categories, AssetState[]? states, string? search, Guid location)
    {
        Query
        .Where(a => !a.IsDeleted)
        .Where(a => a.LocationId == location);  

        if (categories is not null && categories.Length > 0)
            Query.Where(a => categories.Contains(a.Category!.CategoryName));

        if (states is not null && states.Length > 0)
            Query.Where(a => states.Contains(a.State));

        if (search is not null)
        {
            Query.Where(a =>
                a.AssetCode.Replace(" ", "")
                    .ToLower()
                    .Contains(search) ||
                a.Name.Replace(" ", "")
                    .ToLower()
                    .Contains(search));
        }
    }
}