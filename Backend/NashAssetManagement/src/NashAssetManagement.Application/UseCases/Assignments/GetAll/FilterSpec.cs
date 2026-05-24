using Ardalis.Specification;
using NashAssetManagement.Domain.Entities.Core;
using NashAssetManagement.Domain.Enums;

namespace NashAssetManagement.Application.UseCases.Assignments.GetAll
{
    internal class FilterSpec : Specification<Assignment, Response>
    {
        public FilterSpec(Query query) {
            var searchTerm = query.SearchTerm?.Trim();

            Query.Include(x => x.Asset)
                 .Include(x => x.AssignedToUser)
                 .Include(x => x.AssignedByUser)
                 .AsNoTracking()
                 .AsSplitQuery();

            if (!string.IsNullOrWhiteSpace(searchTerm))
            {
                var search = $"%{searchTerm.ToLower()}%";
                Query
                    .Search(x => x.Asset!.AssetCode.ToLower(), search)
                    .Search(x => x.Asset!.Name.ToLower(), search)
                    .Search(x => x.AssignedToUser!.UserName!.ToLower(), search);
            }

            if (!query.IncludeDeleted ?? false)
            {
                Query.Where(x => x.IsDeleted == false);
            }

            if (!string.IsNullOrWhiteSpace(query.State)
            && Enum.TryParse<AssignmentState>(query.State, ignoreCase: true, out var state))
            {
                Query.Where(x => x.State == state);
            }

            if (query.AssignedDate.HasValue)
            {
                var date = query.AssignedDate.Value.Date;
                Query.Where(x => x.AssignedAtUtc.Date == date);
            }

            Query.Select(x => new Response(
               x.Id,
               x.Asset!.AssetCode,
               x.Asset.Name,
               x.AssignedToUser!.UserName?? "",
               x.AssignedByUser!.UserName?? "",
               x.AssignedAtUtc.ToString("dd/MM/yyyy"),
               x.State.ToString()));
        }
    }
}
 