using ErrorOr;
using MediatR;

namespace NashAssetManagement.Application.UseCases.Assets.Delete;

public record DeleteAssetRequest(string Id) : IRequest<ErrorOr<DeleteAssetResponse>>;
