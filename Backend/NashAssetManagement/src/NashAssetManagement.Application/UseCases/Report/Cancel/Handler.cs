using ErrorOr;
using MediatR;
using Microsoft.Extensions.Logging;
using NashAssetManagement.Application.Abstractions.AppIdentity;
using NashAssetManagement.Application.Abstractions.DataAccess;
using NashAssetManagement.Domain.Entities.Jobs.Report;

namespace NashAssetManagement.Application.UseCases.Report.Cancel
{
    public class Handler(
        IRepository<ExportReportJob, Guid> exportReportRepository,
        ICurrentUser user,
        IUnitOfWork uow,
        ILogger<Handler> logger
    ) : IRequestHandler<Request, ErrorOr<Response>>
    {
        public async Task<ErrorOr<Response>> Handle(Request request, CancellationToken cancellationToken)
        {
            if (!user.IsAuthenticated) return Errors.UnauthorizedUser;

            var userId = user.UserId ?? Guid.Empty;
            if (userId == Guid.Empty) return Errors.UnidentifiedUser;

            // Load existing job
            var reportJob = await exportReportRepository.FirstOrDefaultAsync(new ExportReportJobByAdminSpecification(userId), cancellationToken);
            if (reportJob == null)
            {
                return Errors.ReportNotFound;
            }

            // If there's an associated file on disk, delete it
            if (!string.IsNullOrEmpty(reportJob.FilePath))
            {
                var absolutePath = Path.Combine(Directory.GetCurrentDirectory(), reportJob.FilePath);
                if (File.Exists(absolutePath))
                {
                    try
                    {
                        File.Delete(absolutePath);
                        logger.LogInformation("Deleted cancelled export report file at {FilePath}", absolutePath);
                    }
                    catch (Exception ex)
                    {
                        logger.LogError(ex, "Failed to delete report file at {FilePath} during cancellation", absolutePath);
                    }
                }
            }

            // Remove from db
            exportReportRepository.Delete(reportJob);
            await uow.SaveChangesAsync(cancellationToken);

            return new Response();
        }
    }
}
