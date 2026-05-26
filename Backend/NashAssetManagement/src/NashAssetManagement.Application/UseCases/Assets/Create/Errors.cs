using ErrorOr;

namespace NashAssetManagement.Application.UseCases.Assets.Create;

public static class CreateAssetError
{
    public static Error NotFound(Guid id) =>
        Error.NotFound("Asset.NotFound", $"Asset with id '{id}' was not found.");
}