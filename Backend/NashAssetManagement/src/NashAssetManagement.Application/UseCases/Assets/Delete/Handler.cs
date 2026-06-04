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

namespace NashAssetManagement.Application.UseCases.Assets.Delete;

public class DeleteAssetHandler
    : IRequestHandler<DeleteAssetRequest, ErrorOr<DeleteAssetResponse>>
{
    private readonly IRepository<Asset, Guid> _assetRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly DeleteAssetValidator _validator;
    private readonly ICurrentUser _currentUser;
    private readonly IDateTimeProvider _dateTimeProvider;
    private readonly ILogger<DeleteAssetHandler> _logger;

    public DeleteAssetHandler(
        IRepository<Asset, Guid> assetRepository,
        IUnitOfWork unitOfWork,
        DeleteAssetValidator validator,
        ICurrentUser currentUser,
        IDateTimeProvider dateTimeProvider,
        ILogger<DeleteAssetHandler> logger)
    {
        _assetRepository = assetRepository;
        _unitOfWork = unitOfWork;
        _validator = validator;
        _currentUser = currentUser;
        _dateTimeProvider = dateTimeProvider;
        _logger = logger;
    }

    public async Task<ErrorOr<DeleteAssetResponse>> Handle(
        DeleteAssetRequest request,
        CancellationToken cancellationToken)
    {
        var normalizedRequest = request with
        {
            Id = request.Id.Trim(),
        };

        var validationResult = await _validator.ValidateAsync(normalizedRequest, cancellationToken);
        if (!validationResult.IsValid)
            throw new ValidationException(validationResult.Errors);

        Guid location = Guid.TryParse(_currentUser.LocationId, out Guid locationId) ? locationId : Guid.Empty;
        Guid AssetId = Guid.TryParse(request.Id, out Guid assetGuid) ? assetGuid : Guid.Empty;

        if (location == Guid.Empty)
            return DeleteAssetErrors.LocationNotFound;
        if (AssetId == Guid.Empty)
            return DeleteAssetErrors.AssetNotFound;

        var spec = new AssetByIdSpec(AssetId, location);
        var asset = await _assetRepository.FirstOrDefaultAsync(spec, cancellationToken);
        
        if (asset is null)
            return DeleteAssetErrors.AssetNotFound;
        
        if(asset.State == AssetState.Assigned)
            return DeleteAssetErrors.AssetIsAssigned;
        
        if(asset.IsDeleted == true)
            return DeleteAssetErrors.AssetIsDeleted;

        if(asset.Assignments.Any())
            return DeleteAssetErrors.AssetHasHistory;

        try
        {
            asset.DeletedAtUtc = _dateTimeProvider.UtcNow;
            _assetRepository.Delete(asset);
            await _unitOfWork.SaveChangesAsync(cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred while editing the asset.");
            return DeleteAssetErrors.AssetDeleteFailed;
        }

        return new DeleteAssetResponse(
            asset.Id,
            asset.AssetCode,
            asset.Name,
            asset.IsDeleted,
            asset.DeletedAtUtc
        );
    }
}