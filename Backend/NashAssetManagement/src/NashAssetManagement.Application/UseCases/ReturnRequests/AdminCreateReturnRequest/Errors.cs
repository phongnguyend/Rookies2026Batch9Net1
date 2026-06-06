using ErrorOr;

namespace NashAssetManagement.Application.UseCases.ReturnRequests.AdminCreateReturnRequest
{
    internal static class Errors
    {
        public static Error UnauthorizedUser =
           Error.Unauthorized(
               "AdminCreateReturnRequest.UnauthorizedUser",
               "User is not authorized to do this action.");

        public static Error UnidentifiedUser =
            Error.Unauthorized(
                "AdminCreateReturnRequest.UnidentifiedUser",
                "Cannot identify user with user's ID.");

        public static Error AssignmentNotFoundWithId(string assignmentId) =>
            Error.NotFound(
                "AdminCreateReturnRequest.AssignmentNotFoundWithId",
                $"Failed to find assignment with Id '{assignmentId}'.");

        public static Error AssignmentNotSameLocation =
            Error.Conflict(
                "AdminCreateReturnRequest.AssignmentNotSameLocation",
                "Admin can only create return request for assignment in the same location.");

        public static Error AssignmentHasWaitingReturnRequest =
            Error.Conflict(
                "AdminCreateReturnRequest.AssignmentHasWaitingReturnRequest",
                "A pending return request has been created for this assignment.");

        public static Error UnexpectedErrorOccurred =
            Error.Failure(
                "AdminCreateReturnRequest.UnexpectedErrorOccurred",
                "Unexpected error occurred when trying to create return request for admin's assignment. Please try again.");

        public static Error InvalidAssignmentState =
            Error.Validation(
                "AdminCreateReturnRequest.InvalidAssignmentState",
                "Assignment must be in 'Accepted' state in order to create return request.");
    }
}
