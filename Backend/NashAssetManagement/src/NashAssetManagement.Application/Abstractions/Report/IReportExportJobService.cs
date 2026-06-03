namespace NashAssetManagement.Application.Abstractions.Report
{
    public interface IReportExportJobService
    {
        Task CreateJobAsync(Guid exportReportJobId, CancellationToken cancellationToken);
    }
}