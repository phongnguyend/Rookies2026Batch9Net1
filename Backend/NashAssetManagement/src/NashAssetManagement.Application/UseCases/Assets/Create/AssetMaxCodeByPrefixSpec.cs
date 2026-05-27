using Ardalis.Specification;
using NashAssetManagement.Domain.Entities.Core;

namespace NashAssetManagement.Application.UseCases.Assets.Specification;

public sealed class AssetMaxCodeByPrefixSpec : Specification<Asset, string?>
{
    public AssetMaxCodeByPrefixSpec(string prefix)
    {
        Query
            .Where(a => a.AssetCode.StartsWith(prefix))
            .OrderByDescending(a => a.AssetCode);

        Query.Select(a => a.AssetCode);
    }
}