using ErrorOr;

namespace NashAssetManagement.Application.UseCases.Assets.AssetsLookupForEditAssignment
{
    internal static class Errors
    {
        public static Error UnauthorizedUser = Error.Unauthorized(
            "AssetsLookupForEditAssignment.UnauthorizedUser",
            "User is not authorized to do this action.");

        public static Error NotAdminUser = Error.Forbidden(
            "AssetsLookupForEditAssignment.NotAdminUser",
            "Only admin user can do this action.");

        public static Error LocationNotFound = Error.NotFound(
            "AssetsLookupForEditAssignment.LocationNotFound",
            "Cannot find user's location.");

        public static Error InvalidAssignedAssetId = Error.Validation(
            "AssetsLookupForEditAssignment.InvalidAssignedAssetId",
            "Request contains invalid ID of assigned asset.");
    }
}
