using NashAssetManagement.Domain.Entities.Auth;
using NashAssetManagement.Domain.Entities.Identity;

namespace NashAssetManagement.Application.Abstractions.Jwt
{
    public interface IJwtTokenProvider
    {
        string GenerateAccessToken(User user, IList<string> roles);
        RefreshToken GenerateRefreshToken(User user);
    }
}
