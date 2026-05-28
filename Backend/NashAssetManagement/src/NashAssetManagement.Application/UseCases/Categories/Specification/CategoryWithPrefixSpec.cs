using Ardalis.Specification;
using NashAssetManagement.Application.UseCases.Categories.Create;
using NashAssetManagement.Domain.Entities.Core;

namespace NashAssetManagement.Application.UseCases.Categories.Specification;

public sealed class CategoryByIdSpec : Specification<Category, CreateCategoryResponse>
{
    public CategoryByIdSpec(Guid categoryId)
    {
        Query
            .Where(c => c.Id == categoryId)
            .Select(c => new CreateCategoryResponse(
                c.Id,
                c.CategoryName,
                c.Prefix));
    }
}
