using ErrorOr;
namespace NashAssetManagement.Application.UseCases.Auth.ChangePassword
{
    public static class Errors
    {
        public static readonly Error IncorrectOldPassword =
            Error.Validation(
                code: "Auth.IncorrectOldPassword",
                description: "Password is incorrect");

        public static readonly Error UserNotFound =
            Error.NotFound(
                code: "Auth.UserNotFound",
                description: "User not found");
        
        public static readonly Error PasswordDuplicate =
            Error.Conflict(
                code: "Auth.PasswordDuplicate",
                description: "New password must be different from old password");

        public static readonly Error UnexpectedError =
            Error.Unexpected(
                code: "Auth.UnexpectedError",
                description: "An unexpected error occurred while changing password");
        
        public static readonly Error ChangePasswordFailed =
            Error.Failure(
                code: "Auth.ChangePasswordFailed",
                description: "Password change failed");
    }
}
