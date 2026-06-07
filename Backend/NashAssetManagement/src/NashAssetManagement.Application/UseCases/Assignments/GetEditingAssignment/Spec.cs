using Ardalis.Specification;
using NashAssetManagement.Domain.Entities.Core;

namespace NashAssetManagement.Application.UseCases.Assignments.GetEditingAssignment
{
    internal class Spec
        : Specification<Assignment>
    {
        public Spec(Guid assignmentId)
        {
            Query.Where(x => x.Id == assignmentId && !x.IsDeleted)
                .Include(x => x.Asset)
                .ThenInclude(x => x!.Category)
                .Include(x => x.AssignedToUser)
                .AsNoTracking();
        }
    }
}
