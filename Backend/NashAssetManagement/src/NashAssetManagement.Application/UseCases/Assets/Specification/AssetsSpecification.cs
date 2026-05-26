using Ardalis.Specification;
using NashAssetManagement.Application.UseCases.Assets.ViewDetail;
using NashAssetManagement.Application.UseCases.Assets.ViewList;
using NashAssetManagement.Domain.Entities.Core;
using NashAssetManagement.Domain.Enums;

namespace NashAssetManagement.Application.UseCases.Assets.Specification;

public sealed class AssetSpec : Specification<Asset, GetAssetsResponse>
{
    public AssetSpec(string[]? categories, AssetState[]? states, string? search, string? sortBy, string? sortDirection, int pageNumber, int pageSize, Guid location)
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
            Query.Where(a =>
                a.AssetCode.Contains(search) ||
                a.Name.Contains(search));
    }
}

public sealed class AssetDetailSpec
    : Specification<Asset, GetAssetDetailResponse>
{
    public AssetDetailSpec(
        Guid assetId,
        Guid locationId)
    {
        Query
            .Where(x =>
                x.Id == assetId &&
                x.LocationId == locationId &&
                !x.IsDeleted)
            .Include(x => x.Category)
            .Include(x => x.Location)
            .Include(x => x.Assignments)
                .ThenInclude(x => x.AssignedToUser)
            .Include(x => x.Assignments)
                .ThenInclude(x => x.AssignedByUser)
            .Include(x => x.Assignments)
                .ThenInclude(x => x.ReturnRequests)
            .Select(asset => new GetAssetDetailResponse(
                asset.Id,
                asset.AssetCode,
                asset.Name,
                asset.Specification,
                asset.InstalledAtUtc,
                asset.State,
                asset.Category!.CategoryName,
                asset.Location!.Name,
                asset.Assignments
                    .Where(a =>
                        a.State == AssignmentState.Accepted ||
                        a.State == AssignmentState.Returned)
                    .OrderByDescending(a => a.AssignedAtUtc)
                    .Select(a => new GetAssetAssignmentHistoryResponse(
                        a.AssignedAtUtc,
                        $"{a.AssignedToUser!.FirstName} {a.AssignedToUser.LastName}",
                        $"{a.AssignedByUser!.FirstName} {a.AssignedByUser.LastName}",
                        a.State == AssignmentState.Returned
                            ? a.ReturnRequests
                                .OrderByDescending(r => r.ReturnedAtUtc)
                                .Select(r => r.ReturnedAtUtc)
                                .FirstOrDefault()
                            : null
                    ))
                    .ToList()
            ));
    }

    public sealed class CategoryByNameSpec : Specification<Category>
    {
        public CategoryByNameSpec(string name)
        {
            Query.Where(c => c.CategoryName == name);
        }
    }
}