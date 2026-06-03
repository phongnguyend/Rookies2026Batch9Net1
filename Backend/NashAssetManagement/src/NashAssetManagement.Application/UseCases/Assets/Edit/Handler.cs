using ErrorOr;
using FluentValidation;
using MediatR;
using Microsoft.Extensions.Logging;
using NashAssetManagement.Application.Abstractions.AppIdentity;
using NashAssetManagement.Application.Abstractions.DataAccess;
using NashAssetManagement.Application.Abstractions.DateTimes;
using NashAssetManagement.Application.UseCases.Assets.Specification;
using NashAssetManagement.Domain.Entities.Core;
using NashAssetManagement.Domain.Enums;

namespace NashAssetManagement.Application.UseCases.Assets.Edit;

public class EditAssetHandler
    : IRequestHandler<EditAssetRequest, ErrorOr<EditAssetResponse>>
{
    private readonly IRepository<Asset, Guid> _assetRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly EditAssetValidator _validator;
    private readonly ICurrentUser _currentUser;
    private readonly IDateTimeProvider _dateTimeProvider;
    private readonly ILogger<EditAssetHandler> _logger;

    public EditAssetHandler(
        IRepository<Asset, Guid> assetRepository,
        IUnitOfWork unitOfWork,
        EditAssetValidator validator,
        ICurrentUser currentUser,
        IDateTimeProvider dateTimeProvider,
        ILogger<EditAssetHandler> logger)
    {
        _assetRepository = assetRepository;
        _unitOfWork = unitOfWork;
        _validator = validator;
        _currentUser = currentUser;
        _dateTimeProvider = dateTimeProvider;
        _logger = logger;
    }

    public async Task<ErrorOr<EditAssetResponse>> Handle(
        EditAssetRequest request,
        CancellationToken cancellationToken)
    {
        var normalizedRequest = request with
        {
            AssetName = request.AssetName.Trim(),
            Specification = request.Specification.Trim()
        };

        var validationResult = await _validator.ValidateAsync(normalizedRequest, cancellationToken);
        if (!validationResult.IsValid)
            throw new ValidationException(validationResult.Errors);

        Guid location = Guid.TryParse(_currentUser.LocationId, out Guid locationId) ? locationId : Guid.Empty;
        Guid AssetId = Guid.TryParse(request.AssetId, out Guid assetGuid) ? assetGuid : Guid.Empty;

        if (location == Guid.Empty)
            return EditAssetErrors.LocationNotFound;
        if (AssetId == Guid.Empty)
            return EditAssetErrors.AssetNotFound;

        var spec = new AssetByIdSpec(AssetId, location);
        var asset = await _assetRepository.FirstOrDefaultAsync(spec, cancellationToken);
        
        if (asset is null)
            return EditAssetErrors.AssetNotFound;
        
        if(asset.State == AssetState.Assigned)
            return EditAssetErrors.AssetNotEditable;
        
        if(asset.IsDeleted == true)
            return EditAssetErrors.AssetIsSoftDeleted;

        asset.Name = normalizedRequest.AssetName;
        asset.Specification = normalizedRequest.Specification;
        asset.InstalledAtUtc = normalizedRequest.InstalledDate;
        asset.State = normalizedRequest.State;
        asset.UpdatedAtUtc = _dateTimeProvider.UtcNow;

        try
        {
            _assetRepository.UpdateDetached(asset);
            await _unitOfWork.SaveChangesAsync(cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred while editing the asset.");
            return EditAssetErrors.AssetEditFailed;
        }

        return new EditAssetResponse(
            asset.Id,
            asset.AssetCode,
            asset.Name,
            asset.Specification,
            asset.InstalledAtUtc,
            asset.State,
            asset.Category!.CategoryName,
            asset.Location!.Name
        );
    }
}