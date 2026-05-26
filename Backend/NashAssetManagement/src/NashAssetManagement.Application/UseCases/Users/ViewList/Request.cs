using ErrorOr;
using MediatR;
using NashAssetManagement.Application.Utilities;

namespace NashAssetManagement.Application.UseCases.Users.ViewList
{
    public record Request (
        int? PageNumber, 
        int? PageSize, 
        string? SearchTerm, 
        string? Type, 
        string? SortBy, 
        bool? SortDesc
    ) : IRequest<ErrorOr<PagedList<Response>>>;
}