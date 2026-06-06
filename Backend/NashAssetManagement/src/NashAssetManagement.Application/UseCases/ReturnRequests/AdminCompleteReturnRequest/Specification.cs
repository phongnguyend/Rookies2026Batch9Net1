using Ardalis.Specification;
using NashAssetManagement.Domain.Entities.Core;

namespace NashAssetManagement.Application.UseCases.ReturnRequests.AdminCompleteReturnRequest
{
    internal class Specification
        : Specification<ReturnRequest>
    {
        public Specification(Guid returnRequestId)
        {
            Query.Where(x => x.Id == returnRequestId)
                .Include(x => x.Assignment!)
                .ThenInclude(x => x.Asset);
        }
    }
}