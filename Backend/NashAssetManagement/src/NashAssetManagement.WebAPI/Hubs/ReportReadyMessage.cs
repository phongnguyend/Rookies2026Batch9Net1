namespace NashAssetManagement.WebAPI.Hubs
{
    public sealed record ReportReadyMessage(
        string CompletedAtUtc,
        string DownloadUrl
    );
}
