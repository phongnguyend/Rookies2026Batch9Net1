using ErrorOr;

namespace NashAssetManagement.Application.UseCases.Assets.AssetsLookup
{
    internal static class Errors
    {
        public static Error UnauthorizedUser = Error.Unauthorized(
            "AssetsLookup.UnauthorizedUser",
            "User is not authorized to do this action.");

        public static Error NotAdminUser = Error.Forbidden(
            "AssetsLookup.NotAdminUser",
            "Only admin user can do this action.");

        public static Error LocationNotFound = Error.NotFound(
            "AssetsLookup.LocationNotFound",
            "Cannot find user's location.");
    }
}
