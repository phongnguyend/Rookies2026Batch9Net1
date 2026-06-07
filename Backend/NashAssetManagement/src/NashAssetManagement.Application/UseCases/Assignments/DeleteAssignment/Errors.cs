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

        public static Error AssignmentNotFound =
            Error.NotFound(
                "DeleteAssignment.AssignmentNotFound",
                $"Failed to find assignment.");

        public static Error UnexpectedErrorOccurred =
            Error.Failure(
                "DeleteAssignment.UnexpectedErrorOccurred",
                "Unexpected error occurred when trying to delete assignment. Please try again.");

        public static Error InvalidAssignmentState =
            Error.Validation(
                "DeleteAssignment.InvalidAssignmentState",
                "Assignment must be in 'Waiting for acceptance' state in order to be deleted.");

        public static Error AssetOfAssignmentNotFound =
            Error.NotFound(
                "DeleteAssignment.AssetOfAssignmentNotFound",
                $"Failed to find asset of assignment.");

        public static Error InvalidAssignmentAssetState =
            Error.Validation(
                "DeleteAssignment.InvalidAssignmentAssetState",
                "Asset of assignment must be in 'Assigned' state in order to be deleted.");

        public static Error InvalidAssignmentAssetLocation = 
            Error.Validation(
                "DeleteAssignment.InvalidAssignmentAssetLocation",
                "You are not allowed to delete assignment with asset that currently located in different location."
            );

        public static Error AssignmentAlreadyDeleted =
            Error.Validation(
                "DeleteAssignment.AssignmentAlreadyDeleted",
                "This assignment has already been deleted."
            );
    }
}