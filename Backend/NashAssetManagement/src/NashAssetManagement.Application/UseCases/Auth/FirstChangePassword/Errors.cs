using ErrorOr;

namespace NashAssetManagement.Application.UseCases.Auth.FirstChangePassword
{
    public static class Errors
    {
        public static readonly Error UserNotFound = Error.NotFound(
            code: "FirstChangePassword.UserNotFound",
            description: "User was not found.");

        public static readonly Error NotFirstLogin = Error.Conflict(
            code: "FirstChangePassword.NotFirstLogin",
            description: "User has already changed their first password.");

        public static readonly Error ChangePasswordFailed = Error.Conflict(
            code: "FirstChangePassword.Failed",
            description: "Password change failed.");

        public static readonly Error Forbidden = Error.Forbidden(
            code: "FirstChangePassword.Forbidden",
            description: "You do not have permission to change this user's password.");

        public static readonly Error PersistenceFailed = Error.Failure(
            code: "FirstChangePassword.PersistenceFailed",
            description: "Unable to complete password change at this time. Please try again later.");

        public static readonly Error DuplicatePassword = Error.Conflict(
            code: "FirstChangePassword.DuplicatePassword",
            description: "New password must be different from the current password.");
    }
}
