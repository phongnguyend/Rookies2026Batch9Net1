using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using NashAssetManagement.Application.Abstractions.DateTimes;
using NashAssetManagement.Application.Abstractions.Jwt;
using NashAssetManagement.Domain.Constants;
using NashAssetManagement.Domain.Entities.Identity;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
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
                new Claim(JwtTokenConstants.JwtUserIdClaimType, user.Id.ToString()),
                new Claim(JwtTokenConstants.JwtUsernameClaimType, user.UserName!),
                new Claim(JwtTokenConstants.JwtEmailClaimType, user.Email!),
                new Claim(JwtTokenConstants.JwtFirstNameClaimType, user.FirstName!),
                new Claim(JwtTokenConstants.JwtLastNameClaimType, user.LastName!),
                new Claim(JwtTokenConstants.JwtUserTypeClaimType, user.UserType.ToString()),
                new Claim(JwtTokenConstants.JwtJtiClaimType, Guid.NewGuid().ToString())
            };
            foreach (var role in roles)
            {
                claims.Add(new Claim(JwtTokenConstants.JwtRolesClaimType, role));
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

        public string GenerateRefreshToken(User user)
        {
            var claims = new List<Claim>()
            {
                new Claim(JwtTokenConstants.JwtUserIdClaimType, user.Id.ToString()),
                new Claim(JwtTokenConstants.JwtJtiClaimType, Guid.NewGuid().ToString())
            };
            var creds = new SigningCredentials(
                new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtOptions.SecretKey)),
                SecurityAlgorithms.HmacSha256);
            var token = new JwtSecurityToken(
                issuer: _jwtOptions.Issuer,
                audience: _jwtOptions.Audience,
                claims: claims,
                expires: dateTimeProvider.UtcNow.AddMilliseconds(_jwtOptions.RefreshTokenExpiryInMilliseconds),
                signingCredentials: creds);
            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
