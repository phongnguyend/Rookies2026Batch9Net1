using ErrorOr;

namespace NashAssetManagement.Application.UseCases.Assets.ViewDetail;

public static class GetAssetDetailErrors
{
    public static readonly Error NotFoundAssetId = Error.NotFound(
        code: "ViewDetail.NotFoundAssetId",
        description: "Asset with given ID was not found."
    );

    public static readonly Error NotFoundLocation = Error.NotFound(
        code: "ViewDetail.NotFoundLocation",
        description: "Asset's location was not found."
    );

    public static Error AssetNotFound(string assetId) => Error.NotFound(
        code: "ViewDetail.AssetNotFound",
        description: $"Asset with ID '{assetId}' was not found.");
}
