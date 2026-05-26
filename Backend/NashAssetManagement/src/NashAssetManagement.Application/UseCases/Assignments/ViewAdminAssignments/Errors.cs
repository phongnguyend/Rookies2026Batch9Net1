using ErrorOr;

namespace NashAssetManagement.Application.UseCases.Assignments.GetAll
{
    public static class Errors
    {
        public static readonly Error UserNotFound = Error.NotFound(
           "ViewAdminAssignments.UserNotFound",
           "User profile was not found.");

        public static Error UnauthorizedUser = Error.Unauthorized(
           "ViewAdminAssignments.UnauthorizedUser",
           "User is not authorized to do this action.");

        public static Error UnidentifiedUser = Error.Unauthorized(
            "ViewAdminAssignments.UnidentifiedUser",
            "Cannot identify user with user's ID.");
    }
}
