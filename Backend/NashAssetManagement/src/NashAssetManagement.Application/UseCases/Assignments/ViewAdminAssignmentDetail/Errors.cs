using ErrorOr;

namespace NashAssetManagement.Application.UseCases.Assignments.GetById
{
    internal static class Errors
    {
        public static Error AssignmentNotFoundWithId(Guid id) =>
               Error.NotFound(
                   "ViewAdminAssignments.AssignmentNotFoundWithId",
                   $"Assignment with ID '{id}' was not found or it's not belong to your location.");

        public static readonly Error UserNotFound = 
            Error.NotFound(
                "ViewAdminAssignments.UserNotFound",
                "User profile was not found.");

        public static Error UnauthorizedUser =
           Error.Unauthorized(
               "ViewAdminAssignments.UnauthorizedUser",
               "User is not authorized to do this action.");

        public static Error UnidentifiedUser =
            Error.Unauthorized(
                "ViewAdminAssignments.UnidentifiedUser",
                "Cannot identify user with user's ID.");
    }
}
