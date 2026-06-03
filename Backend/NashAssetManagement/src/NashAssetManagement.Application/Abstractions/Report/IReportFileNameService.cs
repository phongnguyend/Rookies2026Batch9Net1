namespace NashAssetManagement.Application.Abstractions.Report
{
    public interface IReportFileNameService
    {
        /// <summary>
        /// File name format: TempReports/HCM_binhnv_report.xlsx
        /// </summary>
        /// <param name="locationName"></param>
        /// <param name="username"></param>
        /// <returns></returns>
        string GenerateStorageFileName(string locationName, string username);

        /// <summary>
        /// File name format: 2026-03-04_HCM_binhnv_report.xlsx
        /// </summary>
        /// <param name="locationName"></param>
        /// <param name="username"></param>
        /// <param name="createdAtUtc"></param>
        /// <returns></returns>
        string GenerateDownloadFileName(string locationName, string username, DateTime createdAtUtc);
    }
}