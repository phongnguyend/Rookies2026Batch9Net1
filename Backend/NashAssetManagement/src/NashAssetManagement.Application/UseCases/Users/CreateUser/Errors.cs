using ErrorOr;

namespace NashAssetManagement.Application.UseCases.Users.CreateUser
{
    internal static class Errors
    {
        public static readonly Error UserNotFound =
            Error.NotFound(
                code: "CreateUser.NotFound",
                description: "User not found.");
                
        public static readonly Error CreateUserFailed =
            Error.Failure(
                code: "CreateUser.CreateFailed",
                description: "Create user failed.");

        public static readonly Error UnexpectedError =
            Error.Unexpected(
                code: "CreateUser.UnexpectedError",
                description: "Failed to create user");
                
        public static readonly Error RoleNotFound =
            Error.NotFound(
                code: "CreateUser.RoleNotFound",
                description: "Role not found.");
    }
}