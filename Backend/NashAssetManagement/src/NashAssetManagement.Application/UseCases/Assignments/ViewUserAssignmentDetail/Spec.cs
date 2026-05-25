using Ardalis.Specification;
using NashAssetManagement.Domain.Entities.Core;

namespace NashAssetManagement.Application.UseCases.Assignments.ViewUserAssignmentDetail
{
    internal class Spec
        : Specification<Assignment, Response>
    {
        public Spec(Guid assignmentId, Guid assigneeId)
        {
            Query.Where(x => x.Id == assignmentId && x.AssignedToUserId == assigneeId)
                .AsNoTracking()
                .Select(x => new Response(
                    x.Id,
                    x.Asset!.AssetCode,
                    x.Asset.Name,
                    x.Asset.Specification,
                    x.AssignedByUser!.UserName!,
                    x.AssignedToUser!.UserName!,
                    x.CreatedAtUtc,
                    x.State.ToString(),
                    x.Note));
        }
    }
}
