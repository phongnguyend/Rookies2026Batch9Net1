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
        string GenerateStorageFileName(string locationName, string username, DateTime createdAtUtc);
    }
}