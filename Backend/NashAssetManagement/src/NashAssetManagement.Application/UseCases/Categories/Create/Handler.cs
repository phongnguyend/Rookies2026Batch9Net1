using ErrorOr;
using FluentValidation;
using MediatR;
using Microsoft.Extensions.Logging;
using NashAssetManagement.Application.Abstractions.DataAccess;
using NashAssetManagement.Application.UseCases.Categories.Specification;
using NashAssetManagement.Domain.Entities.Core;

namespace NashAssetManagement.Application.UseCases.Categories.Create;

public class CreateCategoryHandler
    : IRequestHandler<CreateCategoryRequest, ErrorOr<CreateCategoryResponse>>
{
    private readonly IRepository<Category, Guid> _categoryRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly CreateCategoryValidator _validator;
    private readonly ILogger<CreateCategoryHandler> _logger;

    public CreateCategoryHandler(
        IRepository<Category, Guid> categoryRepository,
        IUnitOfWork unitOfWork,
        CreateCategoryValidator validator,
        ILogger<CreateCategoryHandler> logger)
    {
        _categoryRepository = categoryRepository;
        _unitOfWork = unitOfWork;
        _validator = validator;
        _logger = logger;
    }

    public async Task<ErrorOr<CreateCategoryResponse>> Handle(
        CreateCategoryRequest request,
        CancellationToken cancellationToken)
    {
        var validationResult = await _validator.ValidateAsync(request, cancellationToken);
        if (!validationResult.IsValid)
        {
            throw new ValidationException(validationResult.Errors);
        }

        var categoryExists = await _categoryRepository.AnyAsync(
            new CategoryByNameSpec(request.CategoryName),
            cancellationToken);

        if (categoryExists)
        {
            return CreateCategoryErrors.CategoryAlreadyExists;
        }

        var prefixExists = await _categoryRepository.AnyAsync(
            new CategoryByPrefixSpec(request.CategoryPrefix),
            cancellationToken);

        if (prefixExists)
        {
            return CreateCategoryErrors.PrefixAlreadyExists;
        }

        var category = new Category
        {
            CategoryName = request.CategoryName,
            Prefix = request.CategoryPrefix,
        };
        
        try
        {
            await _categoryRepository.AddAsync(category, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);
        }
        catch
        {
            _logger.LogError("An error occurred while creating the category with name '{CategoryName}' and prefix '{CategoryPrefix}'.", request.CategoryName, request.CategoryPrefix);
            return CreateCategoryErrors.CategoryCreationFailed;
        }

        return new CreateCategoryResponse(
            category.Id,
            category.CategoryName,
            category.Prefix);
    }
}