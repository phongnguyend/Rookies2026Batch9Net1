using System.Text.RegularExpressions;
using Ardalis.Specification;
using NashAssetManagement.Domain.Entities.Core;

namespace NashAssetManagement.Application.UseCases.Assets.AssetsLookupForEditAssignment
{
    internal class FilterSpec
        : Specification<Asset, Response>
    {
        public FilterSpec(Request request, Guid userLocationId)
        {
            Query.Where(x => x.LocationId == userLocationId && !x.IsDeleted);

            if (Guid.TryParse(request.AssignedAssetId, out Guid assignedAssetId))
            {
                Query.Where(x => x.State == Domain.Enums.AssetState.Available || x.Id == assignedAssetId);
            }
            else
            {
                Query.Where(x => x.State == Domain.Enums.AssetState.Available);
            }

            var searchTerm = Regex.Replace(request.SearchTerm?.Trim() ?? "", @"\s+", " ");
            bool hasSearch = !string.IsNullOrWhiteSpace(searchTerm);
            Query.Search(x => x.AssetCode, "%" + searchTerm + "%", hasSearch)
                    .Search(x => x.Name, "%" + searchTerm + "%", hasSearch);

            Query.AsNoTracking()
                .Select(x => new Response(
                    Id: x.Id,
                    AssetCode: x.AssetCode,
                    AssetName: x.Name,
                    Category: x.Category!.CategoryName));
        }
    }
}
