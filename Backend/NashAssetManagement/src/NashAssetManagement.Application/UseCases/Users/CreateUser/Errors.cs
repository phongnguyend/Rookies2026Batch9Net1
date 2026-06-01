using ErrorOr;

namespace NashAssetManagement.Application.UseCases.Users.CreateUser
{
    internal static class Errors
    {
        public static readonly Error UserNotFound =
            Error.NotFound(
                code: "Users.NotFound",
                description: "User not found.");
                
        public static readonly Error CreateUserFailed =
            Error.Failure(
                code: "Users.CreateFailed",
                description: "Create user failed.");

        public static readonly Error UnexpectedError =
            Error.Failure(
                code: "Users.UnexpectedError",
                description: "Failed to create user");
    }
}