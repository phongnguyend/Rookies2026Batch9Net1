using ErrorOr;

namespace NashAssetManagement.Application.UseCases.Assets.ViewDetail;

public static class GetAssetDetailErrors
{
    public static readonly Error NotFoundAssetId = Error.NotFound(
        code:"GetAssetDetail.AssetIsNotFound",
        description:"GetAssetDetail.AssetNotFound"
    );

    public static readonly Error NotFoundLocation = Error.NotFound(
        code:"GetAssetDetail.LocationIsNotFound",
        description: "GetAssetDetail.LocationNotFound"
    );
}
