using ErrorOr;
using MediatR;
using NashAssetManagement.Application.Abstractions.DataAccess;
using NashAssetManagement.Application.Utilities;
using NashAssetManagement.Domain.Entities.Core;

namespace NashAssetManagement.Application.UseCases.Assignments.GetAll
{
    internal class Handler(IRepository<Assignment, Guid> repo)
    : IRequestHandler<Query, ErrorOr<PagedList<Response>>>
    {
        public async Task<ErrorOr<PagedList<Response>>> Handle(
            Query query,
            CancellationToken cancellationToken)
        {
            var filterSpec = new FilterSpec(query);
            var pagingSpec = new PagingSpec(query);

            var totalItems = await repo.CountAsync(filterSpec, cancellationToken);
            var items = await repo.ListAsync(pagingSpec, cancellationToken);

            return PagedList.Create(items, totalItems, query.PageIndex ?? 1, query.PageSize ?? 10);
        }
    }
}
