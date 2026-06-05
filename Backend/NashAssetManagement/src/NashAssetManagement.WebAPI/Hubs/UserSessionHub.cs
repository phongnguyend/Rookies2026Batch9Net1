using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;

namespace NashAssetManagement.WebAPI.Hubs
{
    [Authorize]
    public sealed class UserSessionHub : Hub
    {
    }
}
