using NashAssetManagement.Domain.Enums;

namespace NashAssetManagement.Application.UseCases.Assets.ViewDetail;

public record GetAssetDetailResponse(
    Guid Id,
    string AssetCode,
    string Name,
    string Specification,
    DateTime InstalledAtUtc,
    AssetState State,
    string Category,
    string Location
);
