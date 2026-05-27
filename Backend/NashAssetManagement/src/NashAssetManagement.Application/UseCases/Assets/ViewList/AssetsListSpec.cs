using Ardalis.Specification;
using NashAssetManagement.Application.UseCases.Assets.ViewList;
using NashAssetManagement.Domain.Entities.Core;
using NashAssetManagement.Domain.Enums;

namespace NashAssetManagement.Application.UseCases.Assets.Specification;

public sealed class AssetListSpec : Specification<Asset, GetAssetsResponse>
{
    public AssetListSpec(string[]? categories, AssetState[]? states, string? search, string? sortBy, string? sortDirection, int pageNumber, int pageSize, Guid location)
    {
        var isDesc = sortDirection?.ToLower() == "desc";

        Query
            .Where(a => !a.IsDeleted)
            .Where(a => a.LocationId == location)
            .Include(a => a.Category)
            .Include(a => a.Location)
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize);

        switch (sortBy?.ToLower())
        {
            case "assetcode":
                if (isDesc) Query.OrderByDescending(a => a.AssetCode);
                else Query.OrderBy(a => a.AssetCode);
                break;
            case "name":
                if (isDesc) Query.OrderByDescending(a => a.Name);
                else Query.OrderBy(a => a.Name);
                break;
            case "category":
                if (isDesc) Query.OrderByDescending(a => a.Category!.CategoryName);
                else Query.OrderBy(a => a.Category!.CategoryName);
                break;
            case "state":
                if (isDesc) Query.OrderByDescending(a => a.State);
                else Query.OrderBy(a => a.State);
                break;
            default:
                break;
        }

        if (categories is not null && categories.Length > 0)
            Query.Where(a => categories.Contains(a.Category!.CategoryName));

        if (states is not null && states.Length > 0)
            Query.Where(a => states.Contains(a.State));

        if (search is not null)
            Query.Where(a =>
                a.AssetCode.Contains(search) ||
                a.Name.Contains(search));

        Query.Select(a => new GetAssetsResponse(
            a.Id,
            a.AssetCode,
            a.Name,
            a.Category!.CategoryName,
            a.State,
            a.Location!.Name
        ));
    }
}

public sealed class CategoryByNameSpec : Specification<Category>
{
    public CategoryByNameSpec(string name)
    {
        Query.Where(c => c.CategoryName == name);
    }
}