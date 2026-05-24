using NashAssetManagement.Application.Abstractions.Cookie;

namespace NashAssetManagement.WebAPI.Utilities
{
    public static class HttpResponseExtensions
    {
        public static void AppendAuthCookie(this HttpResponse response, AuthCookie authCookie)
        {
            response.Cookies.Append(
                authCookie.TokenName,
                authCookie.Value,
                new CookieOptions
                {
                    HttpOnly = authCookie.HttpOnly,
                    Secure = authCookie.Secure,
                    IsEssential = authCookie.IsEssential,
                    Path = authCookie.Path,
                    Expires = authCookie.ExpiresAtUtc,
                    SameSite = SameSiteMode.Lax // Set to Lax for avoiding background request
                });
        }
    }
}
