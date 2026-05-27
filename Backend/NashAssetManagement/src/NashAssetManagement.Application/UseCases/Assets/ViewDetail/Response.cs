using NashAssetManagement.Domain.Enums;

namespace NashAssetManagement.Application.UseCases.Assets.ViewDetail;

public record GetAssetAssignmentHistoryResponse(
    DateTime AssignedAtUtc,
    string AssignedTo,
    string AssignedBy,
    DateTime? ReturnedAtUtc);

public record GetAssetDetailResponse(
    Guid Id,
    string AssetCode,
    string Name,
    string Specification,
    DateTime InstalledAtUtc,
    AssetState State,
    string Category,
    string Location,
    IReadOnlyCollection<GetAssetAssignmentHistoryResponse> History
);
