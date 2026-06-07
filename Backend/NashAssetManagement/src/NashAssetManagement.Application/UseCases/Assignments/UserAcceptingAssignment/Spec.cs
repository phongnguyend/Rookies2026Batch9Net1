using Ardalis.Specification;
using NashAssetManagement.Domain.Entities.Core;

namespace NashAssetManagement.Application.UseCases.Assignments.UserAcceptingAssignment
{
    internal class Spec
        : Specification<Assignment>
    {
        public Spec(Guid assignmentId)
        {
            Query.Where(x => x.Id == assignmentId && !x.IsDeleted)
                .Include(x => x.Asset)
                .AsTracking();
        }
    }
}
