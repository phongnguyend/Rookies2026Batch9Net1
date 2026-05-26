using ErrorOr;
using MediatR;
using NashAssetManagement.Application.Utilities;
using NashAssetManagement.Domain.Enums;

namespace NashAssetManagement.Application.UseCases.Assets.ViewList;

public record GetAssetsRequest(
    string[]? Categories,       
    string[]? States,           
    string? SortBy,
    string? SortDirection,
    string? Search,      
    int PageNumber = 1,
    int PageSize = 10      
) : IRequest<ErrorOr<PagedList<GetAssetsResponse>>>;
