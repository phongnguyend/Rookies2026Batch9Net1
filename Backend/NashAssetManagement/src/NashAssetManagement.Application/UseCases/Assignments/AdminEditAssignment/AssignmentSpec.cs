using Ardalis.Specification;
using NashAssetManagement.Domain.Entities.Core;

namespace NashAssetManagement.Application.UseCases.Assignments.AdminEditAssignment
{
    internal class AssignmentSpec
        : Specification<Assignment>
    {
        public AssignmentSpec(Guid assignmentId)
        {
            Query.Where(x => x.Id == assignmentId && !x.IsDeleted)
                .Include(x => x.Asset)
                .Include(x => x.AssignedToUser)
                .Include(x => x.AssignedByUser);
        }
    }
}
