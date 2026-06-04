using ErrorOr;

namespace NashAssetManagement.Application.UseCases.Users.UsersLookup
{
    internal static class Errors
    {
        public static Error UnauthorizedUser = Error.Unauthorized(
            "UsersLookup.UnauthorizedUser",
            "User is not authorized to do this action.");

        public static Error NotAdminUser = Error.Forbidden(
            "UsersLookup.NotAdminUser",
            "Only admin user can do this action.");

        public static Error LocationNotFound = Error.NotFound(
            "UsersLookup.LocationNotFound",
            "Cannot find user's location.");
    }
}
