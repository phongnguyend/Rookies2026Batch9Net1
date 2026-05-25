using System.Security.Claims;

namespace NashAssetManagement.Application.Abstractions.AppIdentity
{
    public interface ICurrentUser
    {
        bool IsAuthenticated { get; }
        Guid? UserId { get; }
        string? Username { get; }
        string? LocationId { get; }
        bool IsFirstTimeLogin { get; }
        IReadOnlyList<string> Roles { get; }
        ClaimsPrincipal Principal { get; }
    }
}
