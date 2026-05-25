using ErrorOr;

namespace NashAssetManagement.Application.UseCases.Auth.Refresh
{
    public static class Errors
    {
        public static readonly Error MissingRefreshToken = Error.Unauthorized(
            code: "Refresh.MissingToken",
            description: "Refresh token cookie is missing.");

        public static readonly Error InvalidRefreshToken = Error.Unauthorized(
            code: "Refresh.InvalidToken",
            description: "Refresh token is invalid.");

        public static readonly Error ExpiredRefreshToken = Error.Unauthorized(
            code: "Refresh.ExpiredToken",
            description: "Refresh token has expired.");

        public static readonly Error RevokedRefreshToken = Error.Unauthorized(
            code: "Refresh.RevokedToken",
            description: "Refresh token has been revoked.");

        public static readonly Error PersistenceFailed = Error.Failure(
            code: "Login.PersistenceFailed",
            description: "Unable to complete login at this time. Please try again later.");
    }
}
