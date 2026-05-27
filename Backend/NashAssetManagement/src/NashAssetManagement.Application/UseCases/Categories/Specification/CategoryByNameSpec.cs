using Ardalis.Specification;
using NashAssetManagement.Application.UseCases.Categories.ViewList;
using NashAssetManagement.Domain.Entities.Core;

namespace NashAssetManagement.Application.UseCases.Categories.Specification;

public sealed class CategoryByNameSpec : Specification<Category>
{
    public CategoryByNameSpec(string name)
    {
        Query.Where(c => c.CategoryName == name);
    }
}