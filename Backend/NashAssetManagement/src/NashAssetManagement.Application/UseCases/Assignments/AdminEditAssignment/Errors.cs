using ErrorOr;

namespace NashAssetManagement.Application.UseCases.Assignments.AdminEditAssignment
{
    internal static class Errors
    {
        public static Error UnauthorizedUser = Error.Unauthorized(
            "AdminEditAssignment.UnauthorizedUser",
            "User is not authorized to do this action.");

        public static Error NotAdminUser = Error.Forbidden(
            "AdminEditAssignment.NotAdminUser",
            "Only admin user can do this action.");

        public static Error LocationNotFound = Error.NotFound(
            "AdminEditAssignment.LocationNotFound",
            "Cannot find user's location.");

        public static Error AssignmentNotFoundWithId(string assignmentId) =>
            Error.NotFound(
                "AdminEditAssignment.AssignmentNotFoundWithId",
                $"Cannot find assignment with Id '{assignmentId}'.");

        public static Error InvalidAssignmentState = Error.Validation(
            "AdminEditAssignment.InvalidAssignmentState",
            "Assignment must be in 'Waiting for acceptance' state to be edited.");

        public static Error CannotEditOtherLocationAssignment = Error.Forbidden(
            "AdminEditAssignment.CannotEditOtherLocationAssignment",
            "Attempted to edit assignment of other location. User can only edit assignment in their location.");

        public static Error UnexpectedErrorOccurred = Error.Failure(
            "AdminEditAssignment.UnexpectedErrorOccurred",
            "Unexpected error occurred when trying to edit assignment. Please try again.");

        public static Error NewUserNotFound(string userId) =>
            Error.NotFound(
                "AdminEditAssignment.NewUserNotFound",
                $"Cannot find user with Id '{userId}' to assign asset to.");

        public static Error CannotAssignUserFromOtherLocation = Error.Forbidden(
            "AdminEditAssignment.CannotAssignUserFromOtherLocation",
            "Attempted to select user from different location to assign asset to. Can only choose user in the same location as admin.");

        public static Error CannotAssignDisabledUser = Error.Forbidden(
            "AdminEditAssignment.CannotAssignDisabledUser",
            "Attempted to select disabled user to assign asset to. Only active user can be assign asset to.");

        public static Error NewAssetNotFound(string assetId) =>
            Error.NotFound(
                "AdminEditAssignment.NewAssetNotFound",
                $"Cannot find asset with Id '{assetId}' to replace current asset of assignment.");

        public static Error InvalidNewAssetState = Error.Validation(
            "AdminEditAssignment.InvalidNewAssetState",
            "Asset must be in 'Available' state to be assigned to user.");
    }
}
