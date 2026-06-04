using ErrorOr;
using FluentValidation;
using Hangfire;
using MediatR;
using Microsoft.Extensions.Logging;
using NashAssetManagement.Application.Abstractions.AppIdentity;
using NashAssetManagement.Application.Abstractions.DataAccess;
using NashAssetManagement.Application.Abstractions.DateTimes;
using NashAssetManagement.Application.Abstractions.Report;
using NashAssetManagement.Domain.Entities.Jobs.Report;
using NashAssetManagement.Domain.Enums;

namespace NashAssetManagement.Application.UseCases.Report.Export
{
    public class Handler(
        IRepository<ExportReportJob, Guid> exportReportRepository,
        ICurrentUser user,
        IUnitOfWork uow,
        ILogger<Handler> logger,
        IValidator<Request> validator,
        IBackgroundJobClient backgroundJobClient,
        IDateTimeProvider dateTimeProvider
    ) : IRequestHandler<Request, ErrorOr<Response>>
    {
        public async Task<ErrorOr<Response>> Handle(Request orgReq, CancellationToken cancellationToken)
        {
            // Validation
            var validationResults = await validator.ValidateAsync(orgReq, cancellationToken);
            if (!validationResults.IsValid) throw new ValidationException(validationResults.Errors);

            // Implement Logic 
            if (!user.IsAuthenticated) return Errors.UnauthorizedUser;

            var userId = user.UserId ?? Guid.Empty;
            var isLocationIdGuid = Guid.TryParse(user.LocationId, out var locationId);
            var userName = user.Username ?? string.Empty;
            if (!isLocationIdGuid || userId == Guid.Empty || string.IsNullOrWhiteSpace(userName)) return Errors.UnidentifiedUser;

            var existingReport = await exportReportRepository.FirstOrDefaultAsync(new ExportReportJobByAdminSpecification(userId), cancellationToken);
            if (existingReport != null)
            {
                if (existingReport.Status == ExportReportJobStatus.Failed)
                {
                    // delete old job if it failed
                    exportReportRepository.Delete(existingReport);
                    await uow.SaveChangesAsync(cancellationToken);
                }
                else
                {
                    logger.LogInformation("User {UserId} attempted to create a new export report while an existing report is still being processed or waiting for download.", userId);
                    return Errors.ReportAlreadyExists;
                }
            }

            var exportReportJob = new ExportReportJob
            {
                RequestedByAdminId = userId,
                Status = ExportReportJobStatus.Processing,
                CreatedAtUtc = dateTimeProvider.UtcNow,
            };

            try
            {
                await exportReportRepository.AddAsync(exportReportJob, cancellationToken);
                await uow.SaveChangesAsync(cancellationToken);

                // Fire excel creation job with the filter
                backgroundJobClient.Enqueue<IReportExportJobService>(service => service.CreateJobAsync(
                    exportReportJob.Id,
                    locationId,
                    userName,
                    orgReq.SortBy ?? ExportReportSortBy.Category,
                    orgReq.SortDirection ?? ExportReportSortDirection.Asc,
                    CancellationToken.None // does not tie with the HTTP request lifecycle
                ));

                return new Response(exportReportJob.Status);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Failed to create export report job for AdminId {AdminId}", user.UserId);
                return Errors.ReportCreationFailed;
            }
        }
    }
}