using Ardalis.Specification;
using NashAssetManagement.Domain.Entities.Core;

namespace NashAssetManagement.Application.UseCases.Categories.Specification;

public sealed class CategoryWithPrefixSpec : Specification<Category, CategoryDto>
{
    public CategoryWithPrefixSpec(string name)
    {
        Query.Where(c => c.CategoryName == name);
        Query.Select(c => new CategoryDto(c.Id, c.CategoryName, c.Prefix));
    }
}

public record CategoryDto(Guid Id, string Name, string Prefix);