using ErrorOr;
using FluentValidation;
using MediatR;
using NashAssetManagement.Application.Abstractions.DataAccess;
using NashAssetManagement.Application.UseCases.Assets.Specification;
using NashAssetManagement.Application.Utilities;
using NashAssetManagement.Domain.Entities.Core;
using NashAssetManagement.Domain.Enums;

namespace NashAssetManagement.Application.UseCases.Assets.ViewList;

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
        await _validator.ValidateAndThrowAsync(request, cancellationToken); // ← validate first

        var stateList = request.States?   // ← parse after validation passes
            .Select(s => Enum.Parse<AssetState>(s))
            .ToArray();

        var countSpec = new AssetCountSpec(request.Categories, stateList);
        var totalCount = await _assetRepository.CountAsync(countSpec, cancellationToken);

        if (totalCount == 0)
            return GetAssetsErrors.NotFound;

        var spec = new AssetSpec(request.Categories, stateList, request.PageNumber, request.PageSize);
        var assets = await _assetRepository.ListAsync(spec, cancellationToken);

        return PagedList.Create(assets, totalCount, request.PageNumber, request.PageSize);
    }
}
