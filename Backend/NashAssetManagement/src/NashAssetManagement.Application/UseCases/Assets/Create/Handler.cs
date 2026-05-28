using ErrorOr;
using FluentValidation;
using MediatR;
using Microsoft.Extensions.Logging;
using NashAssetManagement.Application.Abstractions.AppIdentity;
using NashAssetManagement.Application.Abstractions.DataAccess;
using NashAssetManagement.Application.Abstractions.DateTimes;
using NashAssetManagement.Application.UseCases.Assets.Specification;
using NashAssetManagement.Application.UseCases.Categories.Specification;
using NashAssetManagement.Domain.Entities.Core;

namespace NashAssetManagement.Application.UseCases.Assets.Create;

public class CreateAssetHandler
    : IRequestHandler<CreateAssetRequest, ErrorOr<CreateAssetResponse>>
{
    private readonly IRepository<Asset, Guid> _assetRepository;
    private readonly IRepository<Category, Guid> _categoryRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly CreateAssetValidator _validator;
    private readonly ICurrentUser _currentUser;
    private readonly IDateTimeProvider _dateTimeProvider ;
    private readonly ILogger<CreateAssetHandler> _logger;

    public CreateAssetHandler(
        IRepository<Asset, Guid> assetRepository,
        IRepository<Category, Guid> categoryRepository,
        IUnitOfWork unitOfWork,
        CreateAssetValidator validator,
        ICurrentUser currentUser,
        IDateTimeProvider dateTimeProvider,
        ILogger<CreateAssetHandler> logger)
    {
        _assetRepository = assetRepository;
        _categoryRepository = categoryRepository;
        _unitOfWork = unitOfWork;
        _validator = validator;
        _currentUser = currentUser;
        _dateTimeProvider = dateTimeProvider;
        _logger = logger;
    }

    public async Task<ErrorOr<CreateAssetResponse>> Handle(
        CreateAssetRequest request,
        CancellationToken cancellationToken)
    {
        var normalizedRequest = request with
        {
            AssetName = request.AssetName.Trim(),
            Specification = request.Specification.Trim()
        };
        
        var validationResult = await _validator.ValidateAsync(normalizedRequest, cancellationToken);
        if (!validationResult.IsValid)
        {
            throw new ValidationException(validationResult.Errors);
        }

        if (string.IsNullOrWhiteSpace(_currentUser.LocationId))
        {
            return CreateAssetErrors.LocationNotFound;
        }

        if (!Guid.TryParse(_currentUser.LocationId, out var locationId))
        {
            return CreateAssetErrors.LocationNotFound;
        }

        var category = await _categoryRepository.FirstOrDefaultAsync(
            new CategoryByIdSpec(request.CategoryId),
            cancellationToken);

        if (category is null)
        {
            return CreateAssetErrors.CategoryNotFound;
        }

        var count = await _assetRepository.CountAsync(
            new AssetCountByCategorySpec(category.Id, locationId),
            cancellationToken);
            
        var nextNumber = count + 1;

        if (nextNumber > 999999)
        {
            return CreateAssetErrors.AssetCodeLimitReached;
        }

        var assetCode = $"{category.Prefix}{nextNumber:000000}";

        var asset = new Asset
        {
            Id = Guid.NewGuid(),
            AssetCode = assetCode,
            Name = normalizedRequest.AssetName,
            Specification = normalizedRequest.Specification,
            InstalledAtUtc = request.InstalledDate,
            State = request.State,
            CategoryId = category.Id,
            LocationId = locationId,
            CreatedAtUtc = _dateTimeProvider.UtcNow,
        };

        try
        {
            await _assetRepository.AddAsync(asset, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred while creating the asset.");
            return CreateAssetErrors.AssetCreationFailed;
        }

        return new CreateAssetResponse(
            asset.Id,
            asset.AssetCode,
            asset.Name,
            category.CategoryName,
            asset.State,
            locationId.ToString());
    }
}