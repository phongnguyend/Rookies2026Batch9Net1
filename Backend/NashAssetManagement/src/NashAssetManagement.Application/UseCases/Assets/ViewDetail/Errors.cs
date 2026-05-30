using ErrorOr;

namespace NashAssetManagement.Application.UseCases.Assets.ViewDetail;

public static class GetAssetDetailErrors
{
    public static readonly Error NotFoundAssetId = Error.NotFound(
        code: "GetAssetDetail.NotFoundAssetId",
        description:"Asset with given ID was not found."
    );

    public static readonly Error NotFoundLocation = Error.NotFound(
        code: "GetAssetDetail.NotFoundLocation",
        description: "Asset's location was not found."
    );
}
