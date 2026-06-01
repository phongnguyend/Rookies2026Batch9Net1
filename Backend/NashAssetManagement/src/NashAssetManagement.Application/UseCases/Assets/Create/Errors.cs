using ErrorOr;

namespace NashAssetManagement.Application.UseCases.Assets.Create;

public static class CreateAssetErrors
{
    public static readonly Error CategoryNotFound = Error.NotFound(
        code: "AssetCreate.CategoryNotFound",
        description: "Category not found. Please enter a valid category."
    );

    public static readonly Error AssetCreationFailed = Error.Failure(
        code: "AssetCreate.CreationFailed",
        description: "Failed to create asset. Please try again."
    );

    public static readonly Error AssetCodeLimitReached = Error.Failure(
        code: "AssetCreate.AssetCodeLimitReached",
        description: "Asset code limit has been reached for this category.");

    public static readonly Error LocationNotFound = Error.NotFound(
        code: "AssetCreate.LocationNotFound",
        description: "Location not found. Please enter a valid location."
    );
}