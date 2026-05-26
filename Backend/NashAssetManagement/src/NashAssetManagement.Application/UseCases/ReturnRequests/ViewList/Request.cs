using MediatR;
using ErrorOr;
using NashAssetManagement.Application.Utilities;

namespace NashAssetManagement.Application.UseCases.ReturnRequests.ViewList
{
    public record Request(
        string? SearchTerm,
        List<string>? States,
        string? ReturnedDate,
        string? SortBy,
        bool? SortDesc,
        int? PageSize,
        int? PageNumber
    ) : IRequest<ErrorOr<PagedList<Response>>>;
}