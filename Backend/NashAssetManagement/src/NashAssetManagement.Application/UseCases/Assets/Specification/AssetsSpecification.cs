using Ardalis.Specification;
using NashAssetManagement.Domain.Entities.Core;
using NashAssetManagement.Domain.Enums;

namespace NashAssetManagement.Application.UseCases.Assets.Specification;

public sealed class AssetSpec : Specification<Asset, GetAssetsResponse>
{
    public AssetSpec(string? category, AssetState? state, int pageNumber, int pageSize)
    {
        Query
            .Where(a => !a.IsDeleted)
            // .Where(a => a.Location!.Name == location)  // TODO: uncomment when CurrentUser is ready
            .Include(a => a.Category)
            .Include(a => a.Location)
            .OrderByDescending(a => a.CreatedAtUtc)
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize);

        if (category is not null)
            Query.Where(a => a.Category!.CategoryName == category);

        if (state is not null)
            Query.Where(a => a.State == state);

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

public sealed class AssetDetailSpec : Specification<Asset, GetAssetDetailResponse>
{
    public AssetDetailSpec(Guid id)
    {
        Query
            .Where(a => a.Id == id && !a.IsDeleted)
            .Include(a => a.Category)
            .Include(a => a.Location);

        Query.Select(a => new GetAssetDetailResponse(
            a.Id,
            a.AssetCode,
            a.Name,
            a.Specification,
            a.InstalledAtUtc,
            a.State,
            a.Category!.CategoryName,
            a.Location!.Name
        ));
    }
}

public sealed class AssetCountSpec : Specification<Asset>
{
    public AssetCountSpec(string? category, AssetState? state)
    {
        Query.Where(a => !a.IsDeleted);

        if (category is not null)
            Query.Where(a => a.Category!.CategoryName == category);

        if (state is not null)
            Query.Where(a => a.State == state);
    }
}

public sealed class CategoryByNameSpec : Specification<Category>
{
    public CategoryByNameSpec(string name)
    {
        Query.Where(c => c.CategoryName == name);
    }
}