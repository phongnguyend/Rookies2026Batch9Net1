using System;
using System.Threading;
using System.Threading.Tasks;

namespace NashAssetManagement.Application.Abstractions.Realtime
{
    public interface IUserSessionNotifier
    {
        Task ForceLogoutAsync(Guid userId, string reason, CancellationToken cancellationToken);
        Task NotifyReportReadyAsync(Guid userId, string completedAtUtc, string downloadUrl, CancellationToken cancellationToken);
    }
}
