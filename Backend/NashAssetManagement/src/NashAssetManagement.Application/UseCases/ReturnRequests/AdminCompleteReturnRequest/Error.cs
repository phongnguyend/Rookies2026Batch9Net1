using ErrorOr;

namespace NashAssetManagement.Application.UseCases.ReturnRequests.AdminCompleteReturnRequest
{
    internal static class Errors
    {
        public static readonly Error UserNotFound =
            Error.NotFound(
                code: "AdminCompleteReturnRequest.UserNotFound",
                description: "User not found");

        public static readonly Error ReturnRequestNotFound =
            Error.NotFound(
                code: "AdminCompleteReturnRequest.ReturnRequestNotFound",
                description: "Return request not found");

        public static readonly Error AssignmentNotFound =
            Error.NotFound(
                code: "AdminCompleteReturnRequest.AssignmentNotFound",
                description: "Assignment not found");

        public static readonly Error AssetNotFound =
            Error.NotFound(
                code: "AdminCompleteReturnRequest.AssetNotFound",
                description: "Asset not found");

        public static readonly Error InvalidAssignmentState =
            Error.Conflict(
                code: "AdminCompleteReturnRequest.InvalidAssignmentState",
                description: "Only accepted assignments can be returned");

        public static readonly Error RequestAlreadyCompleted =
            Error.Conflict(
                code: "AdminCompleteReturnRequest.RequestAlreadyCompleted",
                description: "The return request has already been completed");

        public static readonly Error RequestCancelled =
            Error.Conflict(
                code: "AdminCompleteReturnRequest.RequestCancelled",
                description: "Cancelled return requests cannot be completed");
        
        public static readonly Error InvalidRequestState =
            Error.Conflict(
                code: "AdminCompleteReturnRequest.InvalidRequestState",
                description: "Only requests with state 'WaitingForReturning' can be completed");

        public static readonly Error SaveFailed =
            Error.Failure(
                code: "AdminCompleteReturnRequest.SaveFailed",
                description: "Failed to complete returning request");

        public static readonly Error UnexpectedError =
            Error.Unexpected(
                code: "AdminCompleteReturnRequest.UnexpectedError",
                description: "An unexpected error occurred while completing returning request");
    }
}