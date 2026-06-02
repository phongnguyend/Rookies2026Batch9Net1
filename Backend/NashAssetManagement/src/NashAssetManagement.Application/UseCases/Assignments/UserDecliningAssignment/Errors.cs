using ErrorOr;

namespace NashAssetManagement.Application.UseCases.Assignments.UserDecliningAssignment
{
    internal static class Errors
    {
        public static Error UnauthorizedUser =
            Error.Unauthorized(
                "UserDecliningAssignment.UnauthorizedUser",
                "User is not authorized to do this action.");

        public static Error UnidentifiedUser =
            Error.Unauthorized(
                "UserDecliningAssignment.UnidentifiedUser",
                "Cannot identify user with user's ID.");

        public static Error AssignmentNotFoundWithId(string assignmentId) =>
            Error.NotFound(
                "UserDecliningAssignment.AssignmentNotFoundWithId",
                $"Failed to find assignment with Id '{assignmentId}'.");

        public static Error AssignmentNotAssignedToUser =
            Error.Forbidden(
                "UserDecliningAssignment.AssignmentNotAssignedToUser",
                "User can only decline asset that assigned to them.");

        public static Error UnexpectedErrorOccurred =
            Error.Failure(
                "UserDecliningAssignment.UnexpectedErrorOccurred",
                "Unexpected error occurred when trying to decline user's assignment. Please try again.");

        public static Error InvalidAssignmentState =
            Error.Validation(
                "UserDecliningAssignment.InvalidAssignmentState",
                "Assignment must be in 'Waiting for acceptance' state in order to be declined.");

        public static Error AssetOfAssignmentNotFound(string assignmentId) =>
            Error.NotFound(
                "UserDecliningAssignment.AssetOfAssignmentNotFound",
                $"Failed to find asset of assignment with Id '{assignmentId}'.");

        public static Error InvalidAssignmentAssetState =
            Error.Validation(
                "UserDecliningAssignment.InvalidAssignmentAssetState",
                "Asset of assignment must be in 'Assigned' state in order to be declined.");

        public static Error InvalidAssignedDate =
            Error.Validation(
                "UserDecliningAssignment.InvalidAssignedDate",
                "User can only accept assignment that have assigned date equal or earlier than current date.");
    }
}
