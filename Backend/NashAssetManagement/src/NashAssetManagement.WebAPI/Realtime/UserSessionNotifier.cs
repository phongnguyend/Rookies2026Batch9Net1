using Microsoft.AspNetCore.SignalR;
using NashAssetManagement.Application.Abstractions.Realtime;
using NashAssetManagement.WebAPI.Hubs;

namespace NashAssetManagement.WebAPI.Realtime
{
    public sealed class UserSessionNotifier(
        IHubContext<UserSessionHub> hubContext
    ) : IUserSessionNotifier
    {
        public Task ForceLogoutAsync(Guid userId, string reason, CancellationToken cancellationToken)
        {
            return hubContext.Clients
                .User(userId.ToString())
                .SendAsync(
                    UserSessionHubEvents.ForceLogout,
                    new ForceLogoutMessage(reason, DateTime.UtcNow),
                    cancellationToken);
        }

        public Task NotifyReportReadyAsync(Guid userId, string completedAtUtc, string downloadUrl, CancellationToken cancellationToken)
        {
            return hubContext.Clients
                .User(userId.ToString())
                .SendAsync(
                    UserSessionHubEvents.ReportReady,
                    new ReportReadyMessage(completedAtUtc, downloadUrl),
                    cancellationToken);
        }
    }
}
