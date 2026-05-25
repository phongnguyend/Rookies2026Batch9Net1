namespace NashAssetManagement.Application.Abstractions.Cookie
{
    public interface ICookieService
    {
        AuthCookie CreateAccessTokenCookie(string accessToken, DateTime expiresAtUtc);
        AuthCookie CreateRefreshTokenCookie(string refreshToken, DateTime expiresAtUtc);
    }
}