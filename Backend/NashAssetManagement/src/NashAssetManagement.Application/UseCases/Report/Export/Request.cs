using ErrorOr;
using MediatR;
using NashAssetManagement.Domain.Entities.Jobs.Report;

namespace NashAssetManagement.Application.UseCases.Report.Export
{
    // since the options sharing between jobs and services, storing those options at Domain folder for background jobs
    public sealed record Request(
        ExportReportSortDirection? SortDirection = ExportReportSortDirection.Asc,
        ExportReportSortBy? SortBy = ExportReportSortBy.Category
    ) : IRequest<ErrorOr<Response>>;
}