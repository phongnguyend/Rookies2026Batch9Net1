using ErrorOr;
using MediatR;

namespace NashAssetManagement.Application.UseCases.Categories.ViewList;

public record GetCategoriesRequest : IRequest<ErrorOr<List<GetCategoriesResponse>>>;
