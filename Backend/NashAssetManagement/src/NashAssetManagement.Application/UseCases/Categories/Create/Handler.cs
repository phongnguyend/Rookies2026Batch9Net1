using ErrorOr;
using FluentValidation;
using MediatR;
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

    public CreateCategoryHandler(
        IRepository<Category, Guid> categoryRepository,
        IUnitOfWork unitOfWork,
        CreateCategoryValidator validator)
    {
        _categoryRepository = categoryRepository;
        _unitOfWork = unitOfWork;
        _validator = validator;
    }

    public async Task<ErrorOr<CreateCategoryResponse>> Handle(
        CreateCategoryRequest request,
        CancellationToken cancellationToken)
    {
        await _validator.ValidateAndThrowAsync(request, cancellationToken);

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
            Id = Guid.NewGuid(),
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
            return CreateCategoryErrors.CategoryCreationFailed;
        }

        return new CreateCategoryResponse(
            category.Id,
            category.CategoryName,
            category.Prefix);
    }
}