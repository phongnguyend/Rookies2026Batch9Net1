using NashAssetManagement.Domain.Entities.Jobs.Report;

namespace NashAssetManagement.Application.Abstractions.Report
{
    public interface IReportExportJobService
    {
        Task CreateJobAsync(
            Guid exportReportJobId,
            Guid locationId,
            string userName,
            ExportReportSortBy exportReportSortBy,
            ExportReportSortDirection exportReportSortDirection,
            CancellationToken cancellationToken);
    }
}