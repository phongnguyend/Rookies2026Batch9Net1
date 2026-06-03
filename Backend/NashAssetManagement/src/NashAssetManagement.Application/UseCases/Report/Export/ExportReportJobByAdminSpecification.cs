using Ardalis.Specification;
using NashAssetManagement.Domain.Entities.Jobs.Report;

namespace NashAssetManagement.Application.UseCases.Report.Export
{
    public sealed class ExportReportJobByAdminSpecification : Specification<ExportReportJob>
    {
        public ExportReportJobByAdminSpecification(Guid adminId)
        {
            Query
                .Where(x => x.RequestedByAdminId == adminId)
                .OrderByDescending(x => x.CreatedAtUtc)
                .AsNoTracking();
        }
    }
}