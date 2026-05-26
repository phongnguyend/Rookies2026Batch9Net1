using ErrorOr;

namespace NashAssetManagement.Application.UseCases.Assignments.ViewUserAssignments
{
    internal static class Errors
    {
        public static Error UnauthorizedUser = Error.Unauthorized(
            "ViewUserAssignments.UnauthorizedUser",
            "User is not authorized to do this action.");
        public static Error UnidentifiedUser = Error.Unauthorized(
            "ViewUserAssignments.UnidentifiedUser",
            "Cannot identify user with user's ID.");
    }
}
