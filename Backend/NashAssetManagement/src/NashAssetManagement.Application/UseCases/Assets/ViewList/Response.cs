using NashAssetManagement.Domain.Enums;

namespace NashAssetManagement.Application.UseCases.Assets;

public record GetAssetsResponse(
    Guid Id,
    string AssetCode,
    string Name,
    string Category,
    AssetState State,
    string Location
);
