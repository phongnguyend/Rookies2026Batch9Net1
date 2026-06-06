using Ardalis.Specification;
using NashAssetManagement.Domain.Entities.Core;

namespace NashAssetManagement.Application.UseCases.Assignments.GetById
{
    internal class Spec : Specification<Assignment, Response>
    {
        public Spec(Query query, Guid locationId) {
            Query.Where(x => x.Id == query.Id && x.Asset!.LocationId == locationId)
                .Include(x => x.Asset)
                .Include(x => x.AssignedToUser)
                .Include(x => x.AssignedByUser)
                .AsNoTracking()
                .AsSplitQuery()
                .IgnoreQueryFilters();

            Query.Select(x => new Response(
                 x.Id,
                 x.Asset!.AssetCode,
                 x.Asset!.Name,
                 x.Asset!.Specification ?? "",
                 x.AssignedToUser!.UserName ?? "",
                 x.AssignedByUser!.UserName ?? "",
                 x.AssignedAtUtc.ToString("dd/MM/yyyy"),
                 x.State.ToString(),
                 x.Note ?? "",
                 x.IsReturning));
        }
    }
}
