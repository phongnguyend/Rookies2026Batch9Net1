using ErrorOr;

namespace NashAssetManagement.Application.UseCases.Assets.Edit;

public static class EditAssetErrors
{
    public static readonly Error AssetEditFailed = Error.Failure(
        code: "AssetEdit.EditFailed",
        description: "Failed to edit asset. Please try again."
    );

    public static readonly Error LocationNotFound = Error.NotFound(
        code: "AssetEdit.LocationNotFound",
        description: "Location not found. Please enter a valid location."
    );

    public static readonly Error AssetNotFound = Error.NotFound(
        code: "AssetEdit.AssetNotFound",
        description: "Asset not found."
    );

    public static readonly Error AssetNotEditable = Error.Conflict(
        code: "AssetEdit.AssetNotEditable",
        description: "Asset cannot be edited because it is assigned."
    );
}