using Microsoft.AspNetCore.Http;
using NashAssetManagement.Application.Abstractions.AppIdentity;
using NashAssetManagement.Domain.Constants;
using System.Security.Claims;

namespace NashAssetManagement.Infrastructure.AppIdentity
{
    internal class CurrentUser(
        IHttpContextAccessor contextAccessor)
        : ICurrentUser
    {
        private ClaimsPrincipal? User
            => contextAccessor.HttpContext?.User;

        public bool IsAuthenticated
            => User?.Identity?.IsAuthenticated ?? false;

        public Guid? UserId
        {
            get
            {
                var userId = User?.FindFirstValue(JwtTokenConstants.JwtUserIdClaimType);
                return Guid.TryParse(userId, out var guid)
                    ? guid
                    : null;
            }
        }

        public string? Username =>
            User?.FindFirst(JwtTokenConstants.JwtUsernameClaimType)?.Value;

        public string? Email =>
            User?.FindFirst(JwtTokenConstants.JwtEmailClaimType)?.Value;

        public IReadOnlyList<string> Roles =>
            User?.FindAll(JwtTokenConstants.JwtRolesClaimType)
            .Select(x => x.Value)
            .ToList() ?? [];

        public ClaimsPrincipal Principal =>
            User ?? new ClaimsPrincipal();
    }
}
