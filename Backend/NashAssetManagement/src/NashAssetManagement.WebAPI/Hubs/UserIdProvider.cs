using Microsoft.AspNetCore.SignalR;
using NashAssetManagement.Domain.Constants;

namespace NashAssetManagement.WebAPI.Hubs
{
    public sealed class UserIdProvider : IUserIdProvider
    {
        public string? GetUserId(HubConnectionContext connection)
        {
            return connection.User?.FindFirst(JwtTokenConstants.UserId)?.Value;
        }
    }
}
