namespace NashAssetManagement.Application.Abstractions.Report
{
    public record AssetReportRow(
        Guid CategoryId,
        string CategoryName,
        int Total = 0,
        int Assigned = 0,
        int Available = 0,
        int NotAvailable = 0,
        int WaitingForRecycling = 0,
        int Recycled = 0
    );
}