using ErrorOr;

namespace NashAssetManagement.Application.UseCases.Assignments.GetEditingAssignment
{
    internal static class Errors
    {
        public static Error UnauthorizedUser = Error.Unauthorized(
            "GetEditingAssignment.UnauthorizedUser",
            "User is not authorized to do this action.");

        public static Error NotAdminUser = Error.Forbidden(
            "GetEditingAssignment.NotAdminUser",
            "Only admin user can do this action.");

        public static Error LocationNotFound = Error.NotFound(
            "GetEditingAssignment.LocationNotFound",
            "Cannot find user's location.");

        public static Error AssignmentNotFoundWithId(string assignmentId) =>
            Error.NotFound(
                "GetEditingAssignment.AssignmentNotFoundWithId",
                $"Cannot found assignment with Id '{assignmentId}'.");

        public static Error InvalidAssignmentState = Error.Validation(
            "GetEditingAssignment.InvalidAssignmentState",
            "Assignment must be in 'Waiting for acceptance' state to be edited.");

        public static Error CannotEditOtherLocationAssignment = Error.Forbidden(
            "GetEditingAssignment.CannotEditOtherLocationAssignment",
            "Attempted to edit assignment of other location. User can only edit assignment in their location.");
    }
}
