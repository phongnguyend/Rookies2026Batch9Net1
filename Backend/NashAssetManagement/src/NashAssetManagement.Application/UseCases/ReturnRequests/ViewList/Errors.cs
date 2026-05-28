using ErrorOr;

namespace NashAssetManagement.Application.UseCases.ReturnRequests.ViewList
{
    internal static class Errors
    {
        public static Error Unauthorized = Error.Unauthorized(
            "ReturnRequests.ViewList.Unauthorized",
            "Unauthorized to view return requests.");

        public static Error UserHasNoLocation = Error.Conflict(
            "ReturnRequests.ViewList.UserHasNoLocation",
            "User has no location."
        );
    }
}