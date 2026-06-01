using ErrorOr;
using MediatR;

namespace NashAssetManagement.Application.UseCases.Assets.ViewDetail;

public record GetAssetDetailRequest(string? Id) : IRequest<ErrorOr<GetAssetDetailResponse>>;
