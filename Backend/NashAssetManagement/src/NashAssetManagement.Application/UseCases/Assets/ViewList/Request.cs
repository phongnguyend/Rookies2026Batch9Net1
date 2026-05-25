using ErrorOr;
using MediatR;
using NashAssetManagement.Application.Utilities;
using NashAssetManagement.Domain.Enums;

namespace NashAssetManagement.Application.UseCases.Assets.ViewList;

public record GetAssetsRequest(
    string[]? Categories,       // filter by category name e.g. "Laptop"
    string[]? States,      // filter by state e.g. Available
    int PageNumber = 1,
    int PageSize = 10      
) : IRequest<ErrorOr<PagedList<GetAssetsResponse>>>;
