using Ardalis.Specification;
using NashAssetManagement.Domain.Entities.Core;

namespace NashAssetManagement.Application.UseCases.Categories.Specification;

public sealed class CategoryByIdSpec : Specification<Category>
{
    public CategoryByIdSpec(Guid categoryId)
    {
        Query.Where(c => c.Id == categoryId).AsNoTracking();
    }
}
