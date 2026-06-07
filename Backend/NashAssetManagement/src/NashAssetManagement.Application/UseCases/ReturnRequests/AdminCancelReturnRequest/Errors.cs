using ErrorOr;

namespace NashAssetManagement.Application.UseCases.ReturnRequests.AdminCancelReturnRequest
{
    internal static class Errors
    {
        public static Error UnexpectedErrorOccurred =
            Error.Failure(
                "AdminCancelReturnRequest.UnexpectedErrorOccurred",
                "Unexpected error occurred when trying to cancel return request. Please try again.");

        public static Error UnauthorizedUser = Error.Unauthorized(
            "AdminCancelReturnRequest.UnauthorizedUser",
            "User is not authorized to do this action.");

        public static Error NotAdminUser = Error.Forbidden(
            "AdminCancelReturnRequest.NotAdminUser",
            "Only admin user can do this action.");

        public static Error LocationNotFound = Error.NotFound(
            "AdminCancelReturnRequest.LocationNotFound",
            "Cannot find user's location.");

        public static Error RequestNotFound(string requestId) => Error.NotFound(
            "AdminCancelReturnRequest.RequestNotFound",
            $"Cannot find returning request with ID '{requestId}'.");

        public static Error InvalidRequestState =
            Error.Validation(
                "AdminCancelReturnRequest.InvalidRequestState",
                "Returning request must be in 'WaitingForReturning' state in order to be cancelled.");

        public static Error AssignmentNotFound = Error.NotFound(
            "AdminCancelReturnRequest.AssignmentNotFound",
            "Cannot find assignment associated with the returning request.");
    }
}
