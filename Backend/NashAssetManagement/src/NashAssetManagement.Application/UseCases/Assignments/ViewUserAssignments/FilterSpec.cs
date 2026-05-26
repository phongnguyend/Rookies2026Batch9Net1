using Ardalis.Specification;
using NashAssetManagement.Domain.Entities.Core;
using NashAssetManagement.Domain.Enums;

namespace NashAssetManagement.Application.UseCases.Assignments.ViewUserAssignments
{
    internal class FilterSpec
        : Specification<Assignment, Response>
    {
        public FilterSpec(Guid userId, DateTime currentDate, Request request)
        {
            // User Id/Assignee
            Query.Where(x => x.AssignedToUserId == userId);
            // Assignment state
            Query.Where(x => x.State == AssignmentState.WaitingForAcceptance || x.State == AssignmentState.Accepted);
            // Assignment created date
            Query.Where(x => x.CreatedAtUtc <= currentDate);

            Query.AsNoTracking()
                .Select(x => new Response(
                    Id: x.Id,
                    AssetCode: x.Asset!.AssetCode,
                    AssetName: x.Asset.Name,
                    Category: x.Asset.Category!.CategoryName,
                    AssignedDate: x.AssignedAtUtc,
                    State: x.State.ToString(),
                    IsReturning: x.IsReturning));
        }
    }
}
