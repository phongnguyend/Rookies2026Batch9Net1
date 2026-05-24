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
    }
}