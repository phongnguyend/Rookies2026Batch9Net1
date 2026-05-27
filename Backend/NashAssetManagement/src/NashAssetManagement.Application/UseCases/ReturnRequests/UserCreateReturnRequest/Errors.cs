using ErrorOr;

namespace NashAssetManagement.Application.UseCases.ReturnRequests.UserCreateReturnRequest
{
    internal static class Errors
    {
        public static Error UnauthorizedUser =
            Error.Unauthorized(
                "UserCreateReturnRequest.UnauthorizedUser",
                "User is not authorized to do this action.");

        public static Error UnidentifiedUser =
            Error.Unauthorized(
                "UserCreateReturnRequest.UnidentifiedUser",
                "Cannot identify user with user's ID.");

        public static Error AssignmentNotFoundWithId(string assignmentId) =>
            Error.NotFound(
                "UserCreateReturnRequest.AssignmentNotFoundWithId",
                $"Failed to find assignment with Id '{assignmentId}'.");

        public static Error AssignmentNotAssignedToUser =
            Error.Forbidden(
                "UserCreateReturnRequest.AssignmentNotAssignedToUser",
                "User can only create return request for asset assigned to them.");

        public static Error AssignmentHasWaitingReturnRequest =
            Error.Conflict(
                "UserCreateReturnRequest.AssignmentHasWaitingReturnRequest",
                "A pending return request has been created for this assignment.");

        public static Error UnexpectedErrorOccurred =
            Error.Failure(
                "UserCreateReturnRequest.UnexpectedErrorOccurred",
                "Unexpected error occurred when trying to create return request for user's assignment. Please try again.");

        public static Error InvalidAssignmentState =
            Error.Validation(
                "UserCreateReturnRequest.InvalidAssignmentState",
                "Assignment must be in 'Accepted' state in order to create return request.");
    }
}
