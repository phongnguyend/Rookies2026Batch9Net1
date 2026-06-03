using Ardalis.Specification;
using NashAssetManagement.Application.Utilities;
using NashAssetManagement.Domain.Entities.Core;
using System.Linq.Expressions;

namespace NashAssetManagement.Application.UseCases.Assets.AssetsLookup
{
    internal class PagingAndSortingSpec
        : FilterSpec
    {
        const string AssetCodeSortOption = "assetcode";
        const string AssetNameSortOption = "assetname";
        const string CategorySortOption = "category";

        public PagingAndSortingSpec(Request request, Guid userLocationId) : base(request, userLocationId)
        {
            ApplySort(request.SortBy, request.SortDesc);
            Query.ApplyPaging(request.PageNumber ?? 1, request.PageSize ?? 10);
        }

        private void ApplySort(string? sortBy, bool? sortDesc)
        {
            bool descending = sortDesc ?? false;
            Expression<Func<Asset, object>> keySelector = sortBy?.Trim().ToLowerInvariant() switch
            {
                AssetCodeSortOption => x => x.AssetCode,
                AssetNameSortOption => x => x.Name,
                CategorySortOption => x => x.Category!.CategoryName,
                _ => x => x.AssetCode,
            };
            if (descending)
            {
                Query.OrderByDescending(keySelector!);
            }
            else
            {
                Query.OrderBy(keySelector!);
            }
        }
    }
}
