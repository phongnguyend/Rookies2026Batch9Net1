using ErrorOr;
using MediatR;
using NashAssetManagement.Domain.Enums;

namespace NashAssetManagement.Application.UseCases.Assets.Edit;

public record EditAssetRequest(
    string AssetId,
    string AssetName,
    string Specification,
    DateTime InstalledDate,
    AssetState State
) : IRequest<ErrorOr<EditAssetResponse>>;