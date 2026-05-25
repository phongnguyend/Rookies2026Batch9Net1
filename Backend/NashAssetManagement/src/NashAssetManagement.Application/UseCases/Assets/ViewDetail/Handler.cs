using ErrorOr;
using FluentValidation;
using MediatR;
using NashAssetManagement.Application.Abstractions.DataAccess;
using NashAssetManagement.Application.UseCases.Assets.Specification;
using NashAssetManagement.Domain.Entities.Core;

namespace NashAssetManagement.Application.UseCases.Assets.ViewDetail;

public class GetAssetDetailHandler : IRequestHandler<GetAssetDetailRequest, ErrorOr<GetAssetDetailResponse>>
{
    private readonly IRepository<Asset, Guid> _assetRepository;
    private readonly GetAssetDetailValidator _validator;

    public GetAssetDetailHandler(
        IRepository<Asset, Guid> assetRepository,
        GetAssetDetailValidator validator)
    {
        _assetRepository = assetRepository;
        _validator = validator;
    }

    public async Task<ErrorOr<GetAssetDetailResponse>> Handle(
        GetAssetDetailRequest request,
        CancellationToken cancellationToken = default)
    {
        await _validator.ValidateAndThrowAsync(request, cancellationToken);

        var spec = new AssetDetailSpec(request.Id);
        var asset = await _assetRepository.FirstOrDefaultAsync(spec, cancellationToken);

        if (asset is null)
            return GetAssetDetailErrors.NotFound(request.Id);

        return asset;
    }
}
