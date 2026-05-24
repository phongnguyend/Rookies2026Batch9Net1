using Ardalis.Specification;
using NashAssetManagement.Application.Utilities;

namespace NashAssetManagement.Application.UseCases.Assignments.GetAll
{
    internal class PagingSpec : FilterSpec
    {
        public PagingSpec(Query request) : base(request)
        {
            ApplySorting(request.SortBy, request.SortDesc);
            Query.ApplyPaging(request.PageNumber ?? 1, request.PageSize ?? 10);
        }

        #region ApplySorting
        private void ApplySorting(string? sortBy, bool? sortDesc)
        {
            var isDesc = sortDesc ?? false;

            switch (sortBy?.ToLower())
            {
                case "assetcode":
                    if (isDesc) Query.OrderByDescending(x => x.Asset!.AssetCode);
                    else Query.OrderBy(x => x.Asset!.AssetCode);
                    break;

                case "assetname":
                    if (isDesc) Query.OrderByDescending(x => x.Asset!.Name);
                    else Query.OrderBy(x => x.Asset!.Name);
                    break;

                case "assignedto":
                    if (isDesc) Query.OrderByDescending(x => x.AssignedToUser!.UserName);
                    else Query.OrderBy(x => x.AssignedToUser!.UserName);
                    break;

                case "assignedby":
                    if (isDesc) Query.OrderByDescending(x => x.AssignedByUser!.UserName);
                    else Query.OrderBy(x => x.AssignedByUser!.UserName);
                    break;

                case "assigneddate":
                    if (isDesc) Query.OrderByDescending(x => x.AssignedAtUtc);
                    else Query.OrderBy(x => x.AssignedAtUtc);
                    break;

                case "state":
                    if (isDesc) Query.OrderByDescending(x => x.State);
                    else Query.OrderBy(x => x.State);
                    break;

                default:
                    Query.OrderByDescending(x => x.CreatedAtUtc);
                    break;
            }
        }
        #endregion
    }
}
