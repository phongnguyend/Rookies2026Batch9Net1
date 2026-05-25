using ErrorOr;

namespace NashAssetManagement.Application.UseCases.Auth.Login
{
    public static class Errors
    {
        public static readonly Error InvalidCredentials = Error.Unauthorized(
            code: "Login.InvalidCredentials",
            description: "Username or password is incorrect. Please try again."
        );

        public static readonly Error UserIsDisabled = Error.Unauthorized(
            code: "Login.UserIsDisabled",
            description: "User has been disabled.");

        public static Error InvalidCredentialsWithRemainingAttempts(int remainingAttempts)
        {
            return Error.Unauthorized(
                code: "Login.InvalidCredentials",
                description: $"Username or password is incorrect. " + $"{remainingAttempts} attempt(s) remaining."
            );
        }

        public static readonly Error UserLocked = Error.Unauthorized(
            code: "Login.UserLocked",
            description: "Your account has been locked. Please contact administrator."
        );

        public static readonly Error PersistenceFailed = Error.Failure(
            code: "Login.PersistenceFailed",
            description: "Unable to complete login at this time. Please try again later."
        );
    }
}