using NashAssetManagement.Domain.Enums;

namespace NashAssetManagement.Application.UseCases.Assets.Edit;

public record EditAssetResponse(
    Guid Id,
    string AssetCode,
    string AssetName,
    string Specification,
    DateTime InstalledDate,
    AssetState State,
    string Category,
    string Location,
    bool IsDeleted
);