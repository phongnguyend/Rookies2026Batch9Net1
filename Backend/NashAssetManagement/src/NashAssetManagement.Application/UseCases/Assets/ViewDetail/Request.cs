using ErrorOr;
using MediatR;

namespace NashAssetManagement.Application.UseCases.Assets;

public record GetAssetDetailRequest(Guid Id) : IRequest<ErrorOr<GetAssetDetailResponse>>;
