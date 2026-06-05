using Ardalis.Specification;
using NashAssetManagement.Domain.Entities.Core;

namespace NashAssetManagement.Application.UseCases.Assets.Specification;

public sealed class DeleteAssetSpec 
    : Specification<Asset>
{
    public DeleteAssetSpec(
        Guid assetId,
        Guid locationId)
    {
        Query
            .Where(x =>
                x.Id == assetId &&
                x.LocationId == locationId)
            .Include(x => x.Assignments);
    }
}