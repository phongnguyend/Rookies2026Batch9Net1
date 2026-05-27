using ErrorOr;
using FluentValidation;
using MediatR;
using NashAssetManagement.Application.Abstractions.AppIdentity;
using NashAssetManagement.Application.Abstractions.DataAccess;
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

    public CreateAssetHandler(
        IRepository<Asset, Guid> assetRepository,
        IRepository<Category, Guid> categoryRepository,
        IUnitOfWork unitOfWork,
        CreateAssetValidator validator,
        ICurrentUser currentUser)
    {
        _assetRepository = assetRepository;
        _categoryRepository = categoryRepository;
        _unitOfWork = unitOfWork;
        _validator = validator;
        _currentUser = currentUser;
    }

    public async Task<ErrorOr<CreateAssetResponse>> Handle(
        CreateAssetRequest request,
        CancellationToken cancellationToken)
    {
        await _validator.ValidateAndThrowAsync(request, cancellationToken);

        if (string.IsNullOrWhiteSpace(_currentUser.LocationId))
        {
            return CreateAssetErrors.LocationNotFound;
        }

        var locationId = Guid.Parse(_currentUser.LocationId);

        var category = await _categoryRepository.FirstOrDefaultAsync(
            new CategoryWithPrefixSpec(request.CategoryName),
            cancellationToken);

        if (category is null)
        {
            return Error.NotFound(
                "Asset.CategoryNotFound",
                "Category not found.");
        }

        var maxCode = await _assetRepository.FirstOrDefaultAsync(
            new AssetMaxCodeByPrefixSpec(category.Prefix),
            cancellationToken);

        int nextNumber = 1;

        if (!string.IsNullOrWhiteSpace(maxCode))
        {
            var numberPart = maxCode.Replace(category.Prefix, "");

            if (int.TryParse(numberPart, out var currentMax))
            {
                nextNumber = currentMax + 1;
            }
        }

        var assetCode = $"{category.Prefix}{nextNumber:D6}";

        var asset = new Asset
        {
            Id = Guid.NewGuid(),
            AssetCode = assetCode,
            Name = request.AssetName,
            Specification = request.Specification,
            InstalledAtUtc = request.InstalledDate,
            State = request.State,
            CategoryId = category.Id,
            LocationId = locationId,
            CreatedAtUtc = DateTime.UtcNow,
        };

        await _assetRepository.AddAsync(asset, cancellationToken);

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return new CreateAssetResponse(
            asset.Id,
            asset.AssetCode,
            asset.Name,
            category.Name,
            asset.State,
            locationId.ToString());
    }
}