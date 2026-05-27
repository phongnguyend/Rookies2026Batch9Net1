using NashAssetManagement.Domain.Enums;

namespace NashAssetManagement.Application.UseCases.Assets.Create;

public record CreateAssetResponse(
    Guid Id,
    string AssetCode,
    string AssetName,
    string Category,
    AssetState State,
    string Location
);