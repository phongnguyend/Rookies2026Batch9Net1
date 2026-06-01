using Ardalis.Specification;
using NashAssetManagement.Domain.Entities.Core;

namespace NashAssetManagement.Application.UseCases.Assets.Specification;

public sealed class AssetByIdSpec : Specification<Asset>
{
    public AssetByIdSpec(Guid id, Guid locationId)
    {
        Query
            .Where(a => a.Id == id && !a.IsDeleted && a.LocationId == locationId)
            .Include(a => a.Category)
            .Include(a => a.Location);
    }
}