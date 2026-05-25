using ErrorOr;
using MediatR;
using NashAssetManagement.Application.Utilities;
using NashAssetManagement.Domain.Enums;

namespace NashAssetManagement.Application.UseCases.Assets;

public record GetAssetsRequest(
    string? Category,       // filter by category name e.g. "Laptop"
    AssetState? State,      // filter by state e.g. Available
    int PageNumber = 1,
    int PageSize = 10      
) : IRequest<ErrorOr<PagedList<GetAssetsResponse>>>;
