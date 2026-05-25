using Ardalis.Specification;
using NashAssetManagement.Application.UseCases.Categories.ViewList;
using NashAssetManagement.Domain.Entities.Core;

namespace NashAssetManagement.Application.UseCases.Categories.Specification;

public sealed class GetCategoriesSpec : Specification<Category, GetCategoriesResponse>
{
    public GetCategoriesSpec()
    {
        Query
            .OrderBy(c => c.CategoryName);

        Query.Select(c => new GetCategoriesResponse(
            c.Id,
            c.CategoryName
        ));
    }
}