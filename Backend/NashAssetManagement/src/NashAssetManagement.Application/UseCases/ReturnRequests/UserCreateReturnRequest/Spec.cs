using Ardalis.Specification;
using NashAssetManagement.Domain.Entities.Core;

namespace NashAssetManagement.Application.UseCases.ReturnRequests.UserCreateReturnRequest
{
    internal class Spec
        : Specification<Assignment>
    {
        public Spec(Guid assignmentId)
        {
            Query.Where(x => x.Id == assignmentId)
                .Include(x => x.ReturnRequests);
        }
    }
}
