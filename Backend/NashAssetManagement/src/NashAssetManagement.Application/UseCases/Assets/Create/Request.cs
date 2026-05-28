using ErrorOr;
using MediatR;
using NashAssetManagement.Domain.Enums;

namespace NashAssetManagement.Application.UseCases.Assets.Create;

public record CreateAssetRequest(
    string AssetName,
    string Specification,
    DateTime InstalledDate,
    AssetState State,
    Guid CategoryId
) : IRequest<ErrorOr<CreateAssetResponse>>;