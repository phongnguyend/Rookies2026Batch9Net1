using ErrorOr;

namespace NashAssetManagement.Application.UseCases.Users.Disable
{
    internal static class Errors
    {
        public static readonly Error UnauthorizedUser = Error.Unauthorized(
            code: "UserDisable.UnauthorizedUser",
            description: "User is not authorized to do this action.");

        public static readonly Error UnidentifiedUser = Error.Unauthorized(
            code: "UserDisable.UnidentifiedUser",
            description: "Cannot identify the current user.");

        public static readonly Error CannotDisableYourself = Error.Forbidden(
            code: "UserDisable.CannotDisableYourself",
            description: "You cannot disable your own account.");

        public static readonly Error UserNotFound = Error.NotFound(
            code: "UserDisable.UserNotFound",
            description: "User was not found.");

        public static readonly Error UserHasValidAssignments = Error.Conflict(
            code: "UserDisable.UserHasValidAssignments",
            description: "There are valid assignments belonging to this user. Please close all assignments before disabling user.");

        public static readonly Error DisableUserFailed = Error.Failure(
            code: "UserDisable.DisableUserFailed",
            description: "Failed to disable user.");

        public static readonly Error UserAlreadyDisabled = Error.Conflict(
            code: "UserDisable.UserAlreadyDisabled",
            description: "User is already disabled.");
        public static readonly Error UserHasDifferentLocation = Error.Forbidden(
            code: "UserDisable.UserHasDifferentLocation",
            description: "You are not allowed to disable users in a different location.");
    }
}