using NashAssetManagement.Domain.Enums;

namespace NashAssetManagement.Application.UseCases.Assets.Edit;

public record EditAssetResponse(
    Guid Id,
    string AssetCode,
    string AssetName,
    string Category,
    string Specificationm,
    AssetState State,
    string Location
);