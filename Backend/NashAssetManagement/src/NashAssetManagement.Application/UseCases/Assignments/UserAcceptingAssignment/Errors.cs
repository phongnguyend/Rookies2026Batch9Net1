using ErrorOr;

namespace NashAssetManagement.Application.UseCases.Assignments.UserAcceptingAssignment
{
    internal static class Errors
    {
        public static Error UnauthorizedUser =
            Error.Unauthorized(
                "UserAcceptingAssignment.UnauthorizedUser",
                "User is not authorized to do this action.");

        public static Error UnidentifiedUser =
            Error.Unauthorized(
                "UserAcceptingAssignment.UnidentifiedUser",
                "Cannot identify user with user's ID.");

        public static Error AssignmentNotFoundWithId(string assignmentId) =>
            Error.NotFound(
                "UserAcceptingAssignment.AssignmentNotFoundWithId",
                $"Failed to find assignment with Id '{assignmentId}'.");

        public static Error AssignmentNotAssignedToUser =
            Error.Forbidden(
                "UserAcceptingAssignment.AssignmentNotAssignedToUser",
                "User can only decline asset that assigned to them.");

        public static Error UnexpectedErrorOccurred =
            Error.Failure(
                "UserAcceptingAssignment.UnexpectedErrorOccurred",
                "Unexpected error occurred when trying to accept user's assignment. Please try again.");

        public static Error InvalidAssignmentState =
            Error.Validation(
                "UserAcceptingAssignment.InvalidAssignmentState",
                "Assignment must be in 'Waiting for acceptance' state in order to be accepted.");

        public static Error AssetOfAssignmentNotFound(string assignmentId) =>
            Error.NotFound(
                "UserAcceptingAssignment.AssetOfAssignmentNotFound",
                $"Failed to find asset of assignment with Id '{assignmentId}'.");

        public static Error InvalidAssignmentAssetState =
            Error.Validation(
                "UserAcceptingAssignment.InvalidAssignmentAssetState",
                "Asset of assignment must be in 'Assigned' state in order to be accepted.");

        public static Error InvalidAssignedDate =
            Error.Validation(
                "UserAcceptingAssignment.InvalidAssignedDate",
                "User can only accept assignment that have assigned date equal or earlier than current date.");
    }
}
