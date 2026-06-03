using ErrorOr;
using MediatR;
using NashAssetManagement.Application.Utilities;

namespace NashAssetManagement.Application.UseCases.Users.UsersLookup
{
    public record Request(
        string? SearchTerm,
        string? SortBy,
        bool? SortDesc,
        int? PageSize,
        int? PageNumber)
        : IRequest<ErrorOr<PagedList<Response>>>;
}
