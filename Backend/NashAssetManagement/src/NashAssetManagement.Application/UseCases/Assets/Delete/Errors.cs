using ErrorOr;

namespace NashAssetManagement.Application.UseCases.Assets.Delete;

public static class DeleteAssetErrors
{
    public static readonly Error AssetDeleteFailed = Error.Failure(
        code: "AssetDelete.DeleteFailed",
        description: "Failed to Delete asset. Please try again."
    );

    public static readonly Error LocationNotFound = Error.NotFound(
        code: "AssetDelete.LocationNotFound",
        description: "Location not found."
    );

    public static readonly Error AssetNotFound = Error.NotFound(
        code: "AssetDelete.AssetNotFound",
        description: "Asset not found."
    );

    public static readonly Error AssetIsDeleted = Error.Conflict(
        code: "AssetDelete.AssetIsDeleted",
        description: "Asset is deleted. Someone has already deleted it."
    );

    public static readonly Error AssetIsAssigned = Error.Conflict(
        code: "AssetDelete.AssetIsAssigned",
        description: "Asset is assigned. Asset is assigned can not delete it now."
    );

    public static readonly Error AssetHasHistory = Error.Conflict(
        code: "AssetDelete.AssetHasHistory",
        description: "Asset has history. Asset has history can not be deleted."
    );
}