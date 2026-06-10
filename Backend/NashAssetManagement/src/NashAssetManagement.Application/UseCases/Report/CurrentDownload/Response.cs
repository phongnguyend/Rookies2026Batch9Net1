using NashAssetManagement.Domain.Enums;

namespace NashAssetManagement.Application.UseCases.Report.CurrentDownload
{
    public sealed record Response(
        ExportReportJobStatus? Status,
        string? DownloadUrl = null,
        DateTime? CompletedAtUtc = null
    );
}