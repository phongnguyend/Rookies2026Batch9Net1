namespace NashAssetManagement.Application.UseCases.Categories;

public record GetCategoriesResponse(
    Guid Id,
    string Name
);