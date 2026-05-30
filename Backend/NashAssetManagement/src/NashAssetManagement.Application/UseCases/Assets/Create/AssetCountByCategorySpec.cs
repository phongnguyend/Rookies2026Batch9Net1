using Ardalis.Specification;
using NashAssetManagement.Domain.Entities.Core;

namespace NashAssetManagement.Application.UseCases.Assets.Specification;

public sealed class AssetCountByCategorySpec : Specification<Asset>
{
    public AssetCountByCategorySpec(Guid categoryId)
    {
        Query.Where(a => a.CategoryId == categoryId);
    }
}