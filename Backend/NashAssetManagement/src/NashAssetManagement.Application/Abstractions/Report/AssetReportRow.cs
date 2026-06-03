namespace NashAssetManagement.Application.Abstractions.Report
{
    public record AssetReportRow(
        string Category,
        int Assigned = 0,
        int Available = 0,
        int NotAvailable = 0,
        int WaitingForRecycling = 0,
        int Recycled = 0
    );
}