namespace NashAssetManagement.Application.Abstractions.Realtime
{
    public interface IUserSessionNotifier
    {
        Task ForceLogoutAsync(Guid userId, string reason, CancellationToken cancellationToken);
    }
}
