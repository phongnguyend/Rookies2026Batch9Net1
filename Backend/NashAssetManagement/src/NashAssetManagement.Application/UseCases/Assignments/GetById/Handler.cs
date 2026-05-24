using ErrorOr;
using MediatR;
using NashAssetManagement.Application.Abstractions.DataAccess;
using NashAssetManagement.Domain.Entities.Core;

namespace NashAssetManagement.Application.UseCases.Assignments.GetById
{
    internal class Handler(IRepository<Assignment, Guid> repo)
    : IRequestHandler<Query, ErrorOr<Response>>
    {
        public async Task<ErrorOr<Response>> Handle(
            Query query,
            CancellationToken cancellationToken)
        {
            var result = await repo.FirstOrDefaultAsync(new Spec(query), cancellationToken);

            if (result is null)
                return Errors.AssignmentNotFoundWithId(query.Id);

            return result;
        }
    }
}
