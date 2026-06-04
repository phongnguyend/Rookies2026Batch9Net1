using NashAssetManagement.Application.Abstractions.Report;
using NashAssetManagement.Domain.Constants;

namespace NashAssetManagement.Infrastructure.Report
{
    public class ReportFileNameService : IReportFileNameService
    {
        public string GenerateStorageFileName(
               string locationName,
               string username,
               DateTime createdAtUtc)
        {
            return Path.Combine(AppCts.TempFolders.TempReportFolders, $"{createdAtUtc:yyyy-MM-dd}_{locationName}_{username}_report.xlsx");
        }
    }
}