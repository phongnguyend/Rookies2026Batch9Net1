namespace NashAssetManagement.Application.UseCases.Assets.AssetsLookup
{
    public record Response(
        Guid Id,
        string AssetCode,
        string AssetName,
        string Category);
}
