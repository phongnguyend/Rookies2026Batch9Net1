namespace NashAssetManagement.Domain.Constants
{
    public static class JwtTokenConstants
    {
        #region ClaimTypes

        public const string UserId = "userId";
        public const string Username = "username";
        public const string LocationId = "locationId";
        public const string IsFirstLogin = "isFirstLogin";
        public const string Roles = "roles";
        public const string Jti = "jti";

        #endregion

        #region TokenName

        public const string RefreshToken = "refreshToken";
        public const string AccessToken = "accessToken";

        #endregion

        #region CookieName

        public const string CookieAccessToken = "NashAssetManagement.Cookie.AccessToken";
        public const string CookieRefreshToken = "NashAssetManagement.Cookie.RefreshToken";

        #endregion
    }
}
