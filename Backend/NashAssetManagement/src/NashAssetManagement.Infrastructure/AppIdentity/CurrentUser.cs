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
                var userId = User?.FindFirstValue(JwtTokenConstants.UserId);
                return Guid.TryParse(userId, out var guid)
                    ? guid
                    : null;
            }
        }

        public string? Username =>
            User?.FindFirst(JwtTokenConstants.Username)?.Value;

        public IReadOnlyList<string> Roles =>
            User?.FindAll(JwtTokenConstants.Roles)
            .Select(x => x.Value)
            .ToList() ?? [];

        public ClaimsPrincipal Principal =>
            User ?? new ClaimsPrincipal();

        public string? LocationId =>
            User?.FindFirst(JwtTokenConstants.LocationId)?.Value;

        public bool IsFirstTimeLogin
        {
            get
            {
                var value = User?.FindFirst(JwtTokenConstants.IsFirstLogin)?.Value;
                return bool.TryParse(value, out var result) && result;
            }
        }
    }
}
