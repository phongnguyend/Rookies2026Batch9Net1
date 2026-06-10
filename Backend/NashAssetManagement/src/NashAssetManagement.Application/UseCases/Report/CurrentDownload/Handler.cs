using ErrorOr;
using MediatR;
using Microsoft.Extensions.Logging;
using NashAssetManagement.Application.Abstractions.AppIdentity;
using NashAssetManagement.Application.Abstractions.DataAccess;
using NashAssetManagement.Domain.Entities.Jobs.Report;
using NashAssetManagement.Domain.Enums;

namespace NashAssetManagement.Application.UseCases.Report.CurrentDownload
{
    public sealed class Handler(
        IRepository<ExportReportJob, Guid> exportReportRepository,
        ICurrentUser user,
        ILogger<Handler> logger)
        : IRequestHandler<Request, ErrorOr<Response>>
    {
        public async Task<ErrorOr<Response>> Handle(
            Request request,
            CancellationToken cancellationToken)
        {
            // Implement logic
            if (!user.IsAuthenticated) return Errors.UnauthorizedUser;

            var userId = user.UserId ?? Guid.Empty;
            var userName = user.Username ?? string.Empty;
            if (userId == Guid.Empty || string.IsNullOrWhiteSpace(userName)) return Errors.UnidentifiedUser;

            var reportJob = await exportReportRepository.FirstOrDefaultAsync(new ExportReportJobByAdminSpecification(userId), cancellationToken);
            if (reportJob is null)
            {
                logger.LogInformation("No export report found for AdminId {AdminId}, Admin can create new report", userId);
                return new Response(null, null);
            }

            string? downloadUrl = null;
            if (reportJob.Status == ExportReportJobStatus.ReadyToDownload && !string.IsNullOrEmpty(reportJob.FilePath))
            {
                downloadUrl = $"/{reportJob.FilePath.Replace('\\', '/')}";
            }

            return new Response(
                Status: reportJob.Status,
                DownloadUrl: downloadUrl,
                CompletedAtUtc: reportJob.CompletedAtUtc
            );
        }
    }
}