using ErrorOr;
using MediatR;

namespace NashAssetManagement.Application.UseCases.Categories.Create;

public record CreateCategoryRequest(
    string CategoryName,
    string CategoryPrefix)
    : IRequest<ErrorOr<CreateCategoryResponse>>;