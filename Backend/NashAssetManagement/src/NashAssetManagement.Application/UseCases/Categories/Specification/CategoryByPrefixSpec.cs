using Ardalis.Specification;
using NashAssetManagement.Domain.Entities.Core;

namespace NashAssetManagement.Application.UseCases.Categories.Specification;

public sealed class CategoryByPrefixSpec : Specification<Category>
{
    public CategoryByPrefixSpec(string prefix)
    {
        Query.Where(c => c.Prefix == prefix);
    }
}