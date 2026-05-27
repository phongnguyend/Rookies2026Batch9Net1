using ErrorOr;
using FluentValidation;
using MediatR;
using NashAssetManagement.Application.Abstractions.AppIdentity;
using NashAssetManagement.Application.Abstractions.DataAccess;
using NashAssetManagement.Application.UseCases.Assets.Specification;
using NashAssetManagement.Domain.Entities.Core;

namespace NashAssetManagement.Application.UseCases.Assets.ViewDetail;

public class GetAssetDetailHandler
    : IRequestHandler<GetAssetDetailRequest, ErrorOr<GetAssetDetailResponse>>
{
    private readonly IRepository<Asset, Guid> _assetRepository;
    private readonly GetAssetDetailValidator _validator;
    private readonly ICurrentUser _currentUser;

    public GetAssetDetailHandler(
        IRepository<Asset, Guid> assetRepository,
        GetAssetDetailValidator validator,
        ICurrentUser currentUser)
    {
        _assetRepository = assetRepository;
        _validator = validator;
        _currentUser = currentUser;
    }

    public async Task<ErrorOr<GetAssetDetailResponse>> Handle(
        GetAssetDetailRequest request,
        CancellationToken cancellationToken = default)
    {
        await _validator.ValidateAndThrowAsync(
            request,
            cancellationToken);

        var locationId = Guid.Parse(_currentUser.LocationId!);
        var assetId = Guid.Parse(request.Id);
        
        var spec = new AssetDetailSpec(
            assetId,
            locationId);

        var asset = await _assetRepository.FirstOrDefaultAsync(
            spec,
            cancellationToken);

        if (asset is null)
        {
            return GetAssetDetailErrors.NotFound(assetId);
        }

        return asset;
    }
}