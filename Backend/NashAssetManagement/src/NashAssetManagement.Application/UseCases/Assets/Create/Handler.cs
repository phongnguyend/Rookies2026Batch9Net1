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

        // ─── Resolve Category ──────────────────────────
        var categorySpec = new CategoryWithPrefixSpec(request.CategoryName);
        var existingCategory = await _categoryRepository
            .FirstOrDefaultAsync(categorySpec, cancellationToken);

        CategoryDto category;

        if (existingCategory is not null)
        {
            // Category exists — use it directly
            category = existingCategory;
        }
        else
        {
            // New category — prefix is required
            if (string.IsNullOrEmpty(request.CategoryPrefix))
                return CreateAssetErrors.CategoryPrefixRequired;

            // ← Run sequentially to avoid DbContext concurrency issue
            var categoryExists = await _categoryRepository
                .AnyAsync(new CategoryByNameSpec(request.CategoryName), cancellationToken);

            var prefixExists = await _categoryRepository
                .AnyAsync(new CategoryByPrefixSpec(request.CategoryPrefix), cancellationToken);

            // Collect ALL errors at once
            var errors = new List<Error>();

            if (categoryExists)
                errors.Add(CreateAssetErrors.CategoryAlreadyExists);

            if (prefixExists)
                errors.Add(CreateAssetErrors.PrefixAlreadyExists);

            if (errors.Count > 0)
                return errors;

            // Create new category
            var newCategory = new Category
            {
                Id = Guid.NewGuid(),
                CategoryName = request.CategoryName,
                Prefix = request.CategoryPrefix,
            };

            await _categoryRepository.AddAsync(newCategory, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            category = new CategoryDto(
                newCategory.Id,
                newCategory.CategoryName,
                newCategory.Prefix);
        }

        // ─── Generate Asset Code ───────────────────────
        var maxCodeSpec = new AssetMaxCodeByPrefixSpec(category.Prefix);
        var maxCode = await _assetRepository
            .FirstOrDefaultAsync(maxCodeSpec, cancellationToken);

        int nextNumber = 1;
        if (maxCode is not null)
        {
            var numberPart = maxCode.Replace(category.Prefix, "");
            if (int.TryParse(numberPart, out var currentMax))
                nextNumber = currentMax + 1;
        }

        var assetCode = $"{category.Prefix}{nextNumber:D6}";

        // ─── Create Asset ──────────────────────────────
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