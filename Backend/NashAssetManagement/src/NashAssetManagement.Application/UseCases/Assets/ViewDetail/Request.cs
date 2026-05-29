using ErrorOr;
using MediatR;

namespace NashAssetManagement.Application.UseCases.Assets.ViewDetail;

public record GetAssetDetailRequest(Guid Id) : IRequest<ErrorOr<GetAssetDetailResponse>>;
