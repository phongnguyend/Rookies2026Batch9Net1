using ErrorOr;

namespace NashAssetManagement.Application.UseCases.Assets.ViewList;

public static class GetAssetsErrors
{
    public static Error NotFound =>
        Error.NotFound("Asset.NotFound", "No assets were found.");
}
