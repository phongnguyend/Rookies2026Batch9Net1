using Ardalis.Specification;
using NashAssetManagement.Domain.Entities.Core;

namespace NashAssetManagement.Application.UseCases.Assignments.AdminCreateAssignment
{
    internal class AssetSpec : Specification<Asset>
    {
        public AssetSpec(Guid assetId)
        {
            Query.Where(x => x.Id == assetId
            && x.IsDeleted == false);

        }
    }
}
