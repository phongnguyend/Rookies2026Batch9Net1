namespace NashAssetManagement.Application.UseCases.Assets.AssetsLookupForEditAssignment
{
    public record Response(
        Guid Id,
        string AssetCode,
        string AssetName,
        string Category);
}
