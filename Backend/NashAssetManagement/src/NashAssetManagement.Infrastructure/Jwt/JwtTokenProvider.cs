using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using NashAssetManagement.Application.Abstractions.DateTimes;
using NashAssetManagement.Application.Abstractions.Jwt;
using NashAssetManagement.Domain.Constants;
using NashAssetManagement.Domain.Entities.Auth;
using NashAssetManagement.Domain.Entities.Identity;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace NashAssetManagement.Infrastructure.Jwt
{
    internal class JwtTokenProvider(
        IOptions<JwtOptions> options,
        IDateTimeProvider dateTimeProvider)
        : IJwtTokenProvider
    {
        readonly JwtOptions _jwtOptions = options.Value ?? throw new ArgumentNullException(nameof(options));

        public string GenerateAccessToken(User user, IList<string> roles)
        {
            ArgumentNullException.ThrowIfNull(user);
            ArgumentNullException.ThrowIfNull(roles);

            var claims = new List<Claim>
            {
                new Claim(JwtTokenConstants.UserId, user.Id.ToString()),
                new Claim(JwtTokenConstants.Username, user.UserName!),
                new Claim(JwtTokenConstants.LocationId, user.LocationId.ToString()),
                new Claim(JwtTokenConstants.Jti, Guid.NewGuid().ToString()),
                new Claim(JwtTokenConstants.IsFirstLogin, user.IsFirstLogin ? "true" : "false"),
            };

            foreach (var role in roles)
            {
                claims.Add(new Claim(JwtTokenConstants.Roles, role));
            }

            var creds = new SigningCredentials(
                new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtOptions.SecretKey)),
                SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: _jwtOptions.Issuer,
                audience: _jwtOptions.Audience,
                claims: claims,
                expires: dateTimeProvider.UtcNow.AddMilliseconds(_jwtOptions.AccessTokenExpiryInMilliseconds),
                signingCredentials: creds);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        public RefreshToken GenerateRefreshToken(User user)
        {
            return new RefreshToken
            {
                Id = Guid.NewGuid(),
                UserId = user.Id,
                CreatedAtUtc = dateTimeProvider.UtcNow,
                ExpiresAtUtc = dateTimeProvider.UtcNow.AddMilliseconds(_jwtOptions.RefreshTokenExpiryInMilliseconds)
            };
        }
    }
}
