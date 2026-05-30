using ErrorOr;

namespace NashAssetManagement.Application.UseCases.Assets.ViewList;

public static class GetAssetsErrors
{
    public static readonly Error AssetViewList = Error.Failure(
        code: "AssetViewList.FailedGetList",
        description: "Failed to get list assets. Please try again."
    );
}
