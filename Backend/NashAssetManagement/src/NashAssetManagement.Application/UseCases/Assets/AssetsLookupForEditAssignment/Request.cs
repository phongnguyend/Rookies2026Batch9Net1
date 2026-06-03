using ErrorOr;
using MediatR;
using NashAssetManagement.Application.Utilities;

namespace NashAssetManagement.Application.UseCases.Assets.AssetsLookupForEditAssignment
{
    public record Request(
        string? SearchTerm,
        string? SortBy,
        bool? SortDesc,
        int? PageSize,
        int? PageNumber,
        string? AssignedAssetId)
        : IRequest<ErrorOr<PagedList<Response>>>;
}
