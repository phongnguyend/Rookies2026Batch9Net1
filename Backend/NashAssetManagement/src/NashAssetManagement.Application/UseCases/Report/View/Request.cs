using ErrorOr;
using MediatR;
using NashAssetManagement.Domain.Constants;

namespace NashAssetManagement.Application.UseCases.Report.View
{
    public sealed record Request(
        int? PageSize = AppCts.Api.PageSize,
        int? PageNumber = AppCts.Api.PageIndex,
        SortDirection? SortDirection = SortDirection.Asc,
        SortBy? SortBy = SortBy.Category) : IRequest<ErrorOr<Response>>;

    public enum SortBy
    {
        Category,
        Total,
        Assigned,
        Available,
        NotAvailable,
        WaitingForRecycling,
        Recycled
    }

    public enum SortDirection
    {
        Asc,
        Desc
    }
}