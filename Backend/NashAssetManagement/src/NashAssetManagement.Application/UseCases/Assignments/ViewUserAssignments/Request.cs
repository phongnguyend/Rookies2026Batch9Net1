using ErrorOr;
using MediatR;
using NashAssetManagement.Application.Utilities;

namespace NashAssetManagement.Application.UseCases.Assignments.ViewUserAssignments
{
    public record Request(
        string? SortBy,
        bool? SortDesc,
        int? PageSize,
        int? PageNumber)
        : IRequest<ErrorOr<PagedList<Response>>>;
}
