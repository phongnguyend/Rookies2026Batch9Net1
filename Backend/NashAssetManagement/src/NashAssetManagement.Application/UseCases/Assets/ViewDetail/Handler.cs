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
        var validatorResult = await _validator.ValidateAsync(request,cancellationToken);
       
        if (!validatorResult.IsValid)
        {
            throw new ValidationException(validatorResult.Errors);
        }

        Guid locationId =  Guid.TryParse(_currentUser.LocationId, out Guid location) ? location:Guid.Empty;
        Guid AssetCode = Guid.TryParse(request.Id, out Guid AssetCodeGuid) ? AssetCodeGuid:Guid.Empty;
        
        if (locationId == Guid.Empty)
        {
            return GetAssetDetailErrors.NotFoundLocation;
        }
        if (AssetCode == Guid.Empty)
        {
            return GetAssetDetailErrors.NotFoundAssetId;
        }

        var spec = new AssetDetailSpec(
            AssetCode,
            locationId);

        var asset = await _assetRepository.FirstOrDefaultAsync(
            spec,
            cancellationToken);

        if (asset is null)
        {
            return GetAssetDetailErrors.NotFoundAssetId;
        }

        return asset;
    }
}