using Ardalis.Specification;
using NashAssetManagement.Application.Utilities;
using NashAssetManagement.Domain.Entities.Core;
using System.Linq.Expressions;

namespace NashAssetManagement.Application.UseCases.Assignments.ViewUserAssignments
{
    internal class PagingAndSortingSpec
        : FilterSpec
    {
        public PagingAndSortingSpec(Guid userId, DateTime currentDate, Request request)
            : base(userId, currentDate, request)
        {
            ApplySort(request.SortBy, request.SortDesc);
            Query.ApplyPaging(request.PageNumber ?? 1, request.PageSize ?? 20);
        }

        private void ApplySort(string? sortBy, bool? sortDesc)
        {
            bool descending = sortDesc ?? true;
            Expression<Func<Assignment, object>> keySelector = sortBy?.Trim().ToLowerInvariant() switch
            {
                "assetcode" => x => x.Asset!.AssetCode,
                "assetname" => x => x.Asset!.Name,
                "category" => x => x.Asset!.Category!.CategoryName,
                "state" => x => x.State,
                _ => x => x.CreatedAtUtc,
            };
            if (descending)
            {
                Query.OrderByDescending(keySelector!);
            }
            else
            {
                Query.OrderBy(keySelector!);
            }

            //sortBy = sortBy?.Trim().ToLowerInvariant();

            //switch (sortBy)
            //{
            //    case "assetcode":
            //        if (descending)
            //        {
            //            Query.OrderByDescending(x => x.Asset!.AssetCode);
            //        }
            //        else
            //        {
            //            Query.OrderBy(x => x.Asset!.AssetCode);
            //        }
            //        break;

            //    case "assetname":
            //        if (descending)
            //        {
            //            Query.OrderByDescending(x => x.Asset!.Name);
            //        }
            //        else
            //        {
            //            Query.OrderBy(x => x.Asset!.Name);
            //        }
            //        break;

            //    case "category":
            //        if (descending)
            //        {
            //            Query.OrderByDescending(
            //                x => x.Asset!.Category!.CategoryName);
            //        }
            //        else
            //        {
            //            Query.OrderBy(
            //                x => x.Asset!.Category!.CategoryName);
            //        }
            //        break;

            //    case "state":
            //        if (descending)
            //        {
            //            Query.OrderByDescending(x => x.State);
            //        }
            //        else
            //        {
            //            Query.OrderBy(x => x.State);
            //        }
            //        break;

            //    default:
            //        if (descending)
            //        {
            //            Query.OrderByDescending(x => x.CreatedAtUtc);
            //        }
            //        else
            //        {
            //            Query.OrderBy(x => x.CreatedAtUtc);
            //        }
            //        break;
            //}
        }
    }
}
