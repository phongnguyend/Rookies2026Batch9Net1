using NashAssetManagement.Application.Abstractions.Cookie;
using NashAssetManagement.Domain.Constants;

namespace NashAssetManagement.Infrastructure.Cookie
{
    public sealed class CookieService : ICookieService
    {
        public AuthCookie CreateAccessTokenCookie(string accessToken, DateTime expiresAtUtc)
        {
            return new AuthCookie(
                TokenName: JwtTokenConstants.CookieAccessToken,
                Value: accessToken,
                ExpiresAtUtc: expiresAtUtc,
                HttpOnly: true,
                Secure: true,
                Path: "/", // sent to all api requests "/"
                IsEssential: true
            );
        }

        public AuthCookie CreateRefreshTokenCookie(string refreshToken, DateTime expiresAtUtc)
        {
            return new AuthCookie(
                TokenName: JwtTokenConstants.CookieRefreshToken,
                Value: refreshToken,
                ExpiresAtUtc: expiresAtUtc,
                HttpOnly: true,
                Secure: true,
                Path: "/",
                IsEssential: true
            );
        }
    }
}