namespace NashAssetManagement.Domain.Constants
{
    public static class JwtTokenConstants
    {
        #region ClaimTypes

        public const string JwtUserIdClaimType = "userId";
        public const string JwtFirstNameClaimType = "firstName";
        public const string JwtLastNameClaimType = "lastName";
        public const string JwtEmailClaimType = "email";
        public const string JwtUsernameClaimType = "username";
        public const string JwtRolesClaimType = "roles";
        public const string JwtUserTypeClaimType = "userType";

        #endregion

        #region TokenName

        public const string RefreshTokenName = "refreshToken";
        public const string AccessTokenName = "accessToken";

        #endregion
    }
}
