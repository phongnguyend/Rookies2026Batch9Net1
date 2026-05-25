using ErrorOr;
using FluentValidation;
using MediatR;
using NashAssetManagement.Application.Abstractions.DataAccess;
using NashAssetManagement.Application.UseCases.Assets.Specification;
using NashAssetManagement.Application.Utilities;
using NashAssetManagement.Domain.Entities.Core;

namespace NashAssetManagement.Application.UseCases.Assets;

public class GetAssetsHandler : IRequestHandler<GetAssetsRequest, ErrorOr<PagedList<GetAssetsResponse>>>
{
    private readonly IRepository<Asset, Guid> _assetRepository;
    private readonly GetAssetsValidator _validator;
    // private readonly ICurrentUser _currentUser;         

    public GetAssetsHandler(
        IRepository<Asset, Guid> assetRepository,
        GetAssetsValidator validator
    // ICurrentUser curvalidator
    )
    {
        _assetRepository = assetRepository;
        _validator = validator;
        // _currentUser = currentUser;
    }

public async Task<ErrorOr<PagedList<GetAssetsResponse>>> Handle(
        GetAssetsRequest request,
        CancellationToken cancellationToken)
    {
        await _validator.ValidateAndThrowAsync(request, cancellationToken);

        // Count total matching records first
        var countSpec = new AssetCountSpec(request.Category, request.State);
        var totalCount = await _assetRepository.CountAsync(countSpec, cancellationToken);

        if (totalCount == 0)
            return GetAssetsErrors.NotFound;

        // Fetch the current page
        var spec = new AssetSpec(request.Category, request.State, request.PageNumber, request.PageSize);
        var assets = await _assetRepository.ListAsync(spec, cancellationToken);

        return PagedList.Create(assets, totalCount, request.PageNumber, request.PageSize);
    }
}
