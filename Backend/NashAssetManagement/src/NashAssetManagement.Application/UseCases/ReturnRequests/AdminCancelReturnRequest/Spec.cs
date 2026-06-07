using Ardalis.Specification;
using NashAssetManagement.Domain.Entities.Core;

namespace NashAssetManagement.Application.UseCases.ReturnRequests.AdminCancelReturnRequest
{
    internal class Spec
        : Specification<ReturnRequest>
    {
        public Spec(Guid returningRequestId)
        {
            Query
                .Where(x => x.Id == returningRequestId)
                .Include(x => x.Assignment);
        }
    }
}
