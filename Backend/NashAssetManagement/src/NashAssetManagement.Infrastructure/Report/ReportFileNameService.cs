using NashAssetManagement.Application.Abstractions.Report;
using NashAssetManagement.Domain.Constants;

namespace NashAssetManagement.Infrastructure.Report
{
    public class ReportFileNameService : IReportFileNameService
    {
        public string GenerateStorageFileName(
               string locationName,
               string username)
        {
            return Path.Combine(AppCts.TempFolders.TempReportFolders, $"{locationName}_{username}_report.xlsx");
        }

        public string GenerateDownloadFileName(
            string locationName,
            string username,
            DateTime createdAtUtc)
        {
            return $"{createdAtUtc:yyyy-MM-dd}_{locationName}_{username}_report.xlsx";
        }
    }
}