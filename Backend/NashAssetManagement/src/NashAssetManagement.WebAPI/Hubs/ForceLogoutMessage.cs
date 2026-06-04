namespace NashAssetManagement.WebAPI.Hubs
{
    public sealed record ForceLogoutMessage(
        string Reason,
        DateTime OccurredAtUtc
    );
}
