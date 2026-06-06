using ErrorOr;

namespace NashAssetManagement.Application.UseCases.Assignments.DeleteAssignment
{
    internal static class Errors
    {
        public static Error UnauthorizedUser =
            Error.Unauthorized(
                "DeleteAssignment.UnauthorizedUser",
                "User is not authorized to do this action.");

        public static Error UnidentifiedUser =
            Error.Unauthorized(
                "DeleteAssignment.UnidentifiedUser",
                "Cannot identify user with user's ID.");

        public static Error AssignmentNotFoundWithId(string assignmentId) =>
            Error.NotFound(
                "DeleteAssignment.AssignmentNotFoundWithId",
                $"Failed to find assignment with Id '{assignmentId}'.");

        public static Error UnexpectedErrorOccurred =
            Error.Failure(
                "DeleteAssignment.UnexpectedErrorOccurred",
                "Unexpected error occurred when trying to delete assignment. Please try again.");

        public static Error InvalidAssignmentState =
            Error.Validation(
                "DeleteAssignment.InvalidAssignmentState",
                "Assignment must be in 'Waiting for acceptance' state in order to be deleted.");

        public static Error AssetOfAssignmentNotFound(string assignmentId) =>
            Error.NotFound(
                "DeleteAssignment.AssetOfAssignmentNotFound",
                $"Failed to find asset of assignment with Id '{assignmentId}'.");

        public static Error InvalidAssignmentAssetState =
            Error.Validation(
                "DeleteAssignment.InvalidAssignmentAssetState",
                "Asset of assignment must be in 'Assigned' state in order to be deleted.");

        public static Error InvalidAssignmentAssetLocation = 
            Error.Validation(
                "DeleteAssignment.InvalidAssignmentAssetLocation",
                "You are not allowed to delete assignment with asset that currently located in different location."
            );
    }
}