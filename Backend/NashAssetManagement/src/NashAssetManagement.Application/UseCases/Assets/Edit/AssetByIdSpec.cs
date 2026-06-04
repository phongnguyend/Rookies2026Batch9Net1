using System.Security.Cryptography.X509Certificates;
using Ardalis.Specification;
using NashAssetManagement.Domain.Entities.Core;
using NashAssetManagement.Domain.Enums;

namespace NashAssetManagement.Application.UseCases.Assets.Specification;

public sealed class AssetByIdSpec : Specification<Asset>
{
    public AssetByIdSpec(Guid id, Guid locationId)
    {
        Query
            .Where(a => a.Id == id 
                && !a.IsDeleted 
                && a.LocationId == locationId 
                && a.State != AssetState.Assigned)
            .Include(a => a.Category)
            .Include(a => a.Location);
    }
}