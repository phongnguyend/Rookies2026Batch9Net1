using ErrorOr;
using MediatR;

namespace NashAssetManagement.Application.UseCases.Categories;

public record GetCategoriesRequest : IRequest<ErrorOr<List<GetCategoriesResponse>>>;