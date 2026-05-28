using Ardalis.Specification;
using NashAssetManagement.Domain.Entities.Core;

namespace NashAssetManagement.Application.UseCases.ReturnRequests.ViewList
{
    internal class FilterSpec 
        : Specification<ReturnRequest, Response>
    {
        public FilterSpec(Request request, string locationId)
        {
            // Same location with current admin
            Query.Where(x => x.Assignment!.Asset!.LocationId.ToString().Equals(locationId));

            // Search
            if (!string.IsNullOrWhiteSpace(request.SearchTerm))
            {
                var searchTerm = $"%{request.SearchTerm.Trim().ToLower()}%";

                Query
                    .Search(x => x.Assignment!.Asset!.AssetCode.ToLower(), searchTerm)
                    .Search(x => x.Assignment!.Asset!.Name.ToLower(), searchTerm)
                    .Search(x => x.RequestedByUser!.UserName!.ToLower(), searchTerm);
            }

            // Filter with states
            if (request.States is { Count: > 0 })
            {
                Query.Where(x => request.States.Contains(x.State));
            }

            // Filter with returned date
            if (DateTime.TryParse(request.ReturnedDate, out var returnedDate))
            {
                var from = returnedDate.Date;
                var to = from.AddDays(1).AddTicks(-1);

                Query.Where(x =>
                    x.ReturnedAtUtc.HasValue &&
                    x.ReturnedAtUtc.Value >= from &&
                    x.ReturnedAtUtc.Value <= to);
            }

            Query.AsNoTracking()
                .Select(x => new Response
                (
                    x.Id,
                    x.Assignment!.Asset!.AssetCode,
                    x.Assignment!.Asset!.Name,
                    x.RequestedByUser!.UserName!,
                    x.Assignment!.AssignedAtUtc,
                    x.AcceptedByUser != null ? x.AcceptedByUser.UserName : null,
                    x.ReturnedAtUtc,
                    x.State.ToString()
                ));
        }
    }
}
