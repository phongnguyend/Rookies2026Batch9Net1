using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using NashAssetManagement.Application.Abstractions.DataAccess;
using NashAssetManagement.Application.Abstractions.DateTimes;
using NashAssetManagement.Application.Abstractions.File;
using NashAssetManagement.Application.Abstractions.Report;
using NashAssetManagement.Application.UseCases.Report.Export;
using NashAssetManagement.Domain.Constants;
using NashAssetManagement.Domain.Entities.Core;
using NashAssetManagement.Domain.Entities.Jobs.Report;
using NashAssetManagement.Domain.Enums;

namespace NashAssetManagement.Infrastructure.Report
{
    public sealed class ReportExportJobService(
        IRepository<ExportReportJob, Guid> exportRepository,
        IRepository<Category, Guid> categoryRepository,
        IRepository<Location, Guid> locationRepository,
        IDateTimeProvider dateTimeProvider,
        IUnitOfWork uow,
        IReportFileNameService fileNameService,
        [FromKeyedServices(AppCts.Services.ReportExcel)] IExcelGenerator excelReportGenerator,
        ILogger<ReportExportJobService> logger) : IReportExportJobService
    {
        public async Task CreateJobAsync(Guid exportReportJobId, Guid locationId, string userName, ExportReportSortBy exportReportSortBy, ExportReportSortDirection exportReportSortDirection, CancellationToken cancellationToken)
        {
            try
            {
                // Implement data for storing data in excel file
                var categories = await categoryRepository.ListAsync(new ReportCategorySpecification(locationId), cancellationToken);

                var reportRows = categories.Select(category => new AssetReportRow(
                    CategoryId: category.Id,
                    CategoryName: category.CategoryName,
                    Total: category.Assets.Count,
                    Assigned: category.Assets.Count(a => a.State == AssetState.Assigned),
                    Available: category.Assets.Count(a => a.State == AssetState.Available),
                    NotAvailable: category.Assets.Count(a => a.State == AssetState.NotAvailable),
                    WaitingForRecycling: category.Assets.Count(a => a.State == AssetState.WaitingForRecycling),
                    Recycled: category.Assets.Count(a => a.State == AssetState.Recycled)
                )).OrderBy(r => r.CategoryName);

                reportRows = exportReportSortBy switch
                {
                    ExportReportSortBy.Category => exportReportSortDirection == ExportReportSortDirection.Desc
                                        ? reportRows.OrderByDescending(r => r.CategoryName)
                                        : reportRows.OrderBy(r => r.CategoryName),

                    ExportReportSortBy.Total => exportReportSortDirection == ExportReportSortDirection.Desc
                                        ? reportRows.OrderByDescending(r => r.Total)
                                        : reportRows.OrderBy(r => r.Total),

                    ExportReportSortBy.Assigned => exportReportSortDirection == ExportReportSortDirection.Desc
                                        ? reportRows.OrderByDescending(r => r.Assigned)
                                        : reportRows.OrderBy(r => r.Assigned),

                    ExportReportSortBy.Available => exportReportSortDirection == ExportReportSortDirection.Desc
                                        ? reportRows.OrderByDescending(r => r.Available)
                                        : reportRows.OrderBy(r => r.Available),

                    ExportReportSortBy.NotAvailable => exportReportSortDirection == ExportReportSortDirection.Desc
                                        ? reportRows.OrderByDescending(r => r.NotAvailable)
                                        : reportRows.OrderBy(r => r.NotAvailable),

                    ExportReportSortBy.WaitingForRecycling => exportReportSortDirection == ExportReportSortDirection.Desc
                                        ? reportRows.OrderByDescending(r => r.WaitingForRecycling)
                                        : reportRows.OrderBy(r => r.WaitingForRecycling),

                    ExportReportSortBy.Recycled => exportReportSortDirection == ExportReportSortDirection.Desc
                                        ? reportRows.OrderByDescending(r => r.Recycled)
                                        : reportRows.OrderBy(r => r.Recycled),

                    _ => reportRows.OrderBy(r => r.CategoryName)
                };

                var items = reportRows.ToList();

                // Generate Excel Bytes + Metadata
                var excelBytes = excelReportGenerator.Generate(
                    items,
                    "Asset Report"
                );

                // Get Location Name
                // - Currently throw and handle as job failed due to asynchronous response to user
                var location = await locationRepository.GetQueryableSet().Where(l => l.Id == locationId).FirstOrDefaultAsync(cancellationToken);
                if (location == null)
                {
                    throw new InvalidOperationException($"Location not found for export excel.");
                }

                // Update the status of export if success
                var exportJob = await exportRepository.GetQueryableSet().Where(e => e.Id == exportReportJobId).FirstOrDefaultAsync(cancellationToken);
                if (exportJob == null)
                {
                    // currently throw error and mark the job as Failed, not notify user
                    throw new InvalidOperationException($"Export report job {exportReportJobId} not found");
                }

                var excelFileName = fileNameService.GenerateStorageFileName(location.Name, userName, exportJob.CreatedAtUtc);

                // Create temp directiory to store the excel file
                var absolutePathToStore = Path.Combine(Directory.GetCurrentDirectory(), excelFileName);
                Directory.CreateDirectory(Path.GetDirectoryName(absolutePathToStore)!);

                await File.WriteAllBytesAsync(absolutePathToStore, excelBytes, cancellationToken);

                // Update job info in db
                exportJob.FilePath = excelFileName;
                exportJob.Status = ExportReportJobStatus.ReadyToDownload;
                exportJob.CompletedAtUtc = dateTimeProvider.UtcNow;

                await uow.SaveChangesAsync(cancellationToken);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Failed to generate report {JobId}", exportReportJobId);
                var exportJob = await exportRepository.GetQueryableSet()
                                    .Where(e => e.Id == exportReportJobId)
                                    .FirstOrDefaultAsync(cancellationToken: cancellationToken);

                if (exportJob != null)
                {
                    exportJob.Status = ExportReportJobStatus.Failed;
                    await uow.SaveChangesAsync(CancellationToken.None);
                }
            }
        }
    }
}