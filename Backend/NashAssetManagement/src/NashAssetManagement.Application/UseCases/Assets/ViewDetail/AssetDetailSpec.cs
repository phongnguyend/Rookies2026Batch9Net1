using Ardalis.Specification;
using NashAssetManagement.Application.UseCases.Assets.ViewDetail;
using NashAssetManagement.Domain.Entities.Core;
using NashAssetManagement.Domain.Enums;

namespace NashAssetManagement.Application.UseCases.Assets.Specification;

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
}