using NashAssetManagement.Domain.Entities.Identity;

namespace NashAssetManagement.Application.Abstractions.Jwt
{
    public interface IJwtTokenProvider
    {
        string GenerateAccessToken(User user, IList<string> roles);
        string GenerateRefreshToken(User user);
    }
}
