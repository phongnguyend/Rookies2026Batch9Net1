using ErrorOr;

namespace NashAssetManagement.Application.UseCases.Assets.ViewDetail;

public static class GetAssetDetailErrors
{
    public static Error NotFound(Guid id) =>
        Error.NotFound("Asset.NotFound", $"Asset with id '{id}' was not found.");
}
