using Ardalis.Specification;
using ErrorOr;
using MediatR;
using NashAssetManagement.Application.Abstractions.DataAccess;
using NashAssetManagement.Domain.Entities.Core;

namespace NashAssetManagement.Application.UseCases.Categories.GetCategories;

public class GetCategoriesHandler
    : IRequestHandler<GetCategoriesRequest, ErrorOr<List<GetCategoriesResponse>>>
{
    private readonly IRepository<Category, Guid> _categoryRepository;

    public GetCategoriesHandler(IRepository<Category, Guid> categoryRepository)
    {
        _categoryRepository = categoryRepository;
    }

    public async Task<ErrorOr<List<GetCategoriesResponse>>> Handle(
        GetCategoriesRequest request,
        CancellationToken cancellationToken)
    {
        var spec = new GetCategoriesSpec();
        var categories = await _categoryRepository.ListAsync(spec, cancellationToken);

        return categories;
    }
}

// Spec is so simple, just put it in the same file
file sealed class GetCategoriesSpec : Specification<Category, GetCategoriesResponse>
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