using ErrorOr;

namespace NashAssetManagement.Application.UseCases.Auth.Logout
{
    public static class Errors
    {
        public static readonly Error InvalidRefreshToken =
            Error.Validation(
                code: "Auth.InvalidRefreshToken",
                description: "Invalid refresh token");

        public static readonly Error UnexpectedError =
            Error.Failure(
                code: "Auth.UnexpectedError",
                description: "Failed to logout");
        
        public static readonly Error Unauthorized =
            Error.Unauthorized(
                code: "Auth.Unauthorized",
                description: "User is not authenticated");
    }
}
