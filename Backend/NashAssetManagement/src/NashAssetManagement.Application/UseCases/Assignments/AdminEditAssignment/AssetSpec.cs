using Ardalis.Specification;
using NashAssetManagement.Domain.Entities.Core;

namespace NashAssetManagement.Application.UseCases.Assignments.AdminEditAssignment
{
    internal class AssetSpec
        : Specification<Asset>
    {
        public AssetSpec(Guid assetId, Guid locationId)
        {
            Query.Where(x => x.Id == assetId && !x.IsDeleted && x.LocationId == locationId);
        }
    }
}
