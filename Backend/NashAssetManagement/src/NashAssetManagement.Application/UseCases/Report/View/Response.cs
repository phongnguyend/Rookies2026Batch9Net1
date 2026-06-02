using NashAssetManagement.Application.Utilities;

namespace NashAssetManagement.Application.UseCases.Report.View
{
    // record only inherit from other record
    // using class to avoid that behavior
    public sealed class Response(
        List<ReportRow> items,
        int count,
        int pageNumber,
        int pageSize) : PagedList<ReportRow>(items, count, pageNumber, pageSize);

    public sealed record ReportRow(
        Guid CategoryId,
        string CategoryName,
        int Total,
        int Assigned,
        int Available,
        int NotAvailable,
        int WaitingForRecycling,
        int Recycled
    );
}