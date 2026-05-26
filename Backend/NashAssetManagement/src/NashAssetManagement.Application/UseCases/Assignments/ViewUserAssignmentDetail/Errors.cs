using ErrorOr;

namespace NashAssetManagement.Application.UseCases.Assignments.ViewUserAssignmentDetail
{
    internal static class Errors
    {
        public static Error UnauthorizedUser =
            Error.Unauthorized(
                "ViewUserAssignmentDetail.UnauthorizedUser",
                "User is not authorized to do this action.");
        public static Error UnidentifiedUser =
            Error.Unauthorized(
                "ViewUserAssignments.UnidentifiedUser",
                "Cannot identify user with user's ID.");
        public static Error AssignmentNotFoundWithId(string assignmentId) =>
            Error.NotFound(
                "ViewUserAssignmentDetail.AssignmentNotFoundWithId",
                $"Failed to find assignment with Id '{assignmentId}'.");
    }
}
