using ErrorOr;

namespace NashAssetManagement.Application.UseCases.Assets.Create;

public static class CreateAssetErrors
{
    public static readonly Error CategoryNotFound = Error.NotFound(
        code: "Asset.CategoryNotFound",
        description: "Category not found. Please enter a valid category."
    );

    public static readonly Error AssetCreationFailed = Error.Failure(
        code: "Asset.CreationFailed",
        description: "Failed to create asset. Please try again."
    );

    public static readonly Error InvalidAssetCodeFormat = Error.Failure(
        code: "Asset.InvalidAssetCodeFormat",
        description: "Existing asset code format is invalid.");

    public static readonly Error AssetCodeLimitReached = Error.Failure(
        code: "Asset.AssetCodeLimitReached",
        description: "Asset code limit has been reached for this category.");

    public static readonly Error LocationNotFound = Error.NotFound(
        code: "Asset.LocationNotFound",
        description: "Location not found. Please enter a valid location."
    );
}