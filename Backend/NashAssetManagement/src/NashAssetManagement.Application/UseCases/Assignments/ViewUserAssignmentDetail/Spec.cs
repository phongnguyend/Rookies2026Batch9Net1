using Ardalis.Specification;
using NashAssetManagement.Domain.Entities.Core;

namespace NashAssetManagement.Application.UseCases.Assignments.ViewUserAssignmentDetail
{
    internal class Spec
        : Specification<Assignment, Response>
    {
        public Spec(Guid assignmentId, Guid assigneeId)
        {
            Query.Where(x => x.Id == assignmentId && x.AssignedToUserId == assigneeId && !x.IsDeleted)
                .AsNoTracking()
                .Select(x => new Response(
                    AssignmentId: x.Id,
                    AssetCode: x.Asset!.AssetCode,
                    AssetName: x.Asset.Name,
                    Specification: x.Asset.Specification,
                    AssignerName: x.AssignedByUser!.UserName!,
                    AssigneeName: x.AssignedToUser!.UserName!,
                    AssignedDate: x.AssignedAtUtc,
                    State: x.State.ToString(),
                    Note: x.Note));
        }
    }
}
