namespace NashAssetManagement.Application.UseCases.Categories.ViewList;

public record GetCategoriesResponse(
    Guid Id,
    string Name,
    string prefix
);
