namespace NashAssetManagement.Application.UseCases.Categories.Create;

public record CreateCategoryResponse(
    Guid Id,
    string Name,
    string Prefix);