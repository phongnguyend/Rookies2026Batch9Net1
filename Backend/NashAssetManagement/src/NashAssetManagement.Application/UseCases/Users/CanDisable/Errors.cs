using ErrorOr;

namespace NashAssetManagement.Application.UseCases.Users.CanDisable
{
    internal static class Errors
    {
        public static readonly Error UnauthorizedUser = Error.Unauthorized(
            code: "UserCanDisable.UnauthorizedUser",
            description: "User is not authorized to do this action.");

        public static readonly Error UnidentifiedUser = Error.Unauthorized(
            code: "UserCanDisable.UnidentifiedUser",
            description: "Cannot identify the current user.");

        public static readonly Error UserNotFound = Error.NotFound(
            code: "UserCanDisable.UserNotFound",
            description: "User was not found.");

        public static readonly Error OnlyAdminCanDisableUser = Error.Forbidden(
            code: "UserCanDisable.OnlyAdminCanDisableUser",
            description: "Only administrators can disable users.");

        public static readonly Error CheckUserDisableFailed = Error.Failure(
            code: "UserCanDisable.CheckUserDisableFailed",
            description: "Failed to check user disable status.");
    }
}