using ErrorOr;
using MediatR;
using NashAssetManagement.Application.Utilities;

namespace NashAssetManagement.Application.UseCases.Assets.ViewList;

public record GetAssetsRequest(
    string? Categories,       
    string? States,           
    string? SortBy,
    string? SortDirection,
    string? Search,
    bool isCreatedNewAsset,      
    int PageNumber = 1,
    int PageSize = 10      
) : IRequest<ErrorOr<PagedList<GetAssetsResponse>>>;
