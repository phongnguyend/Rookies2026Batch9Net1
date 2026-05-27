using NashAssetManagement.Application.Abstractions.Cookie;

namespace NashAssetManagement.WebAPI.Utilities
{
    public static class HttpResponseExtensions
    {
        public static void AppendAuthCookie(this HttpResponse response, AuthCookie authCookie)
        {
            // var host = response.HttpContext.Request.Host.Host;
            // var isLocalhost = host == "localhost" || host == "127.0.0.1";

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
                    // SameSite = isLocalhost ? SameSiteMode.None : SameSiteMode.Lax
                    SameSite = SameSiteMode.None
                });
        }

        public static void DeleteAcccessCookie(this HttpResponse response, string tokenName)
        {
            response.Cookies.Delete(tokenName, new CookieOptions
            {
                HttpOnly = true,
                Secure = true,
                IsEssential = true,
                Path = "/",
                SameSite = SameSiteMode.None
            });
        }

        public static void DeleteRefreshCookie(this HttpResponse response, string tokenName)
        {
            response.Cookies.Delete(tokenName, new CookieOptions
            {
                HttpOnly = true,
                Secure = true,
                IsEssential = true,
                Path = "/",
                SameSite = SameSiteMode.None
            });
        }
    }
}
