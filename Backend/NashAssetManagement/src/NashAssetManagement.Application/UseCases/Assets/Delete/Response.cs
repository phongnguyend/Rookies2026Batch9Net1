using NashAssetManagement.Domain.Enums;

namespace NashAssetManagement.Application.UseCases.Assets.Delete;

public sealed record DeleteAssetResponse(
    Guid Id,
    string AssetCode,
    string AssetName,
    bool isDeleted,
    DateTime? DeletedTime
);