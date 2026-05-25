using ErrorOr;

namespace NashAssetManagement.Application.UseCases.Users.ViewList
{
    internal static class Errors
    {
        public static Error Unauthorized() => 
            Error.Unauthorized(
                "Users.Unauthorized", 
                "User is unauthorized.");

        public static Error UserNotFound() =>
            Error.NotFound(
                "Users.NotFound",
                "User not found.");  

        public static Error UserHasNoLocation() =>
            Error.NotFound(
                "Users.NotFound",
                "User has no location assigned.");   
    }
}