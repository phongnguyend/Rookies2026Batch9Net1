using NashAssetManagement.Domain.Constants;

namespace NashAssetManagement.Application.UseCases.Report.View;

internal static class ReportQueryExtensions
{
    public static IQueryable<ReportRow> ApplySorting(
        this IQueryable<ReportRow> query,
        SortBy? sortBy,
        SortDirection? sortDirection)
    {
        bool descending = sortDirection == SortDirection.Desc;

        return sortBy switch
        {
            SortBy.Category => descending
                ? query.OrderByDescending(x => x.CategoryName)
                : query.OrderBy(x => x.CategoryName),

            SortBy.Total => descending
                ? query.OrderByDescending(x => x.Total)
                : query.OrderBy(x => x.Total),

            SortBy.Assigned => descending
                ? query.OrderByDescending(x => x.Assigned)
                : query.OrderBy(x => x.Assigned),

            SortBy.Available => descending
                ? query.OrderByDescending(x => x.Available)
                : query.OrderBy(x => x.Available),

            SortBy.NotAvailable => descending
                ? query.OrderByDescending(x => x.NotAvailable)
                : query.OrderBy(x => x.NotAvailable),

            SortBy.WaitingForRecycling => descending
                ? query.OrderByDescending(x => x.WaitingForRecycling)
                : query.OrderBy(x => x.WaitingForRecycling),

            SortBy.Recycled => descending
                ? query.OrderByDescending(x => x.Recycled)
                : query.OrderBy(x => x.Recycled),

            _ => query.OrderBy(x => x.CategoryName)
        };
    }

    public static IQueryable<ReportRow> ApplyPaging(
        this IQueryable<ReportRow> query,
        int pageNumber = AppCts.Api.PageIndex,
        int pageSize = AppCts.Api.PageSize)
    {
        return query
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize);
    }
}