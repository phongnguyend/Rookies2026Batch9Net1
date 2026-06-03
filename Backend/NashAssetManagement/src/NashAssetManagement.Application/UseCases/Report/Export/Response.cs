namespace NashAssetManagement.Application.UseCases.Report.Export
{
    public sealed record Response(Domain.Enums.ExportReportJobStatus status);
}