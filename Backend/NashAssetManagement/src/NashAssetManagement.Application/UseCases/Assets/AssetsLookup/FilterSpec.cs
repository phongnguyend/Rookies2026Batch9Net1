using Ardalis.Specification;
using NashAssetManagement.Domain.Entities.Core;

namespace NashAssetManagement.Application.UseCases.Assets.AssetsLookup
{
    internal class FilterSpec
        : Specification<Asset, Response>
    {
        public FilterSpec(Request request, Guid userLocationId)
        {
            Query.Where(x => x.LocationId == userLocationId
                && x.State == Domain.Enums.AssetState.Available
                && !x.IsDeleted);
            if (!string.IsNullOrWhiteSpace(request.SearchTerm))
            {
                var searchTerm = request.SearchTerm.Trim();
                Query.Search(x => x.AssetCode, "%" + searchTerm + "%")
                    .Search(x => x.Name, "%" + searchTerm + "%");
            }
            Query.AsNoTracking()
                .Select(x => new Response(
                    Id: x.Id,
                    AssetCode: x.AssetCode,
                    AssetName: x.Name,
                    Category: x.Category!.CategoryName));
        }
    }
}
