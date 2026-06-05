using Ardalis.Specification;
using NashAssetManagement.Domain.Entities.Core;

namespace NashAssetManagement.Application.UseCases.Assets.Specification;

public sealed class AssetByIdSpec : Specification<Asset>
{
    public AssetByIdSpec(
        Guid id,
        Guid locationId)
    {
        Query
            .Where(a =>
                a.Id == id &&
                a.LocationId == locationId &&
                !a.IsDeleted)

            .Include(a => a.Category)
            .Include(a => a.Location)
            .Include(a => a.Assignments);
    }
}