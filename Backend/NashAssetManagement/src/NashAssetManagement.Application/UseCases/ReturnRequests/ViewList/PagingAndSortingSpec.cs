using System.Linq.Expressions;
using Ardalis.Specification;
using NashAssetManagement.Application.Utilities;
using NashAssetManagement.Domain.Entities.Core;

namespace NashAssetManagement.Application.UseCases.ReturnRequests.ViewList
{
    internal class PagingAndSortingSpec
        : FilterSpec
    {
        public PagingAndSortingSpec(Request request, string locationId) 
            : base(request, locationId)
        {
            ApplySort(request.SortBy, request.SortDesc);
            Query.ApplyPaging(request.PageNumber ?? 1, request.PageSize ?? 20);
        }

        private void ApplySort(string? sortBy, bool? sortDesc)
        {
            bool descending = sortDesc ?? true;
            // cSpell:words assetcode assetname requestedby assigneddate acceptedby returneddate
            Expression<Func<ReturnRequest, object?>> keySelector 
                = sortBy?.Trim().ToLowerInvariant() switch
            {
                "assetcode" => x => x.Assignment!.Asset!.AssetCode,
                "assetname" => x => x.Assignment!.Asset!.Name,
                "requestedby" => x => x.RequestedByUser!.UserName,
                "assigneddate" => x => x.Assignment!.AssignedAtUtc,
                "acceptedby" => x => x.AcceptedByUser != null ? x.AcceptedByUser.UserName : null,
                "returneddate" => x => x.ReturnedAtUtc,
                "state" => x => x.State,
                _ => x => x.CreatedAtUtc
            };
            if (descending)
            {
                Query.OrderByDescending(keySelector);
            }
            else
            {
                Query.OrderBy(keySelector);
            }
        }
    }
}
