using NashAssetManagement.Application.Abstractions.Report;

namespace NashAssetManagement.Infrastructure.Report
{
    public class ReportFileNameService : IReportFileNameService
    {
        public string GenerateStorageFileName(
               string locationName,
               string username)
        {
            return $"{locationName}_{username}_report.xlsx";
        }

        public string GenerateDownloadFileName(
            string locationName,
            string username,
            DateTime createdAtUtc)
        {
            return
                $"{createdAtUtc:yyyy-MM-dd}_{locationName}_{username}_report.xlsx";
        }
    }
}