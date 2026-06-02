using ErrorOr;

namespace NashAssetManagement.Application.UseCases.Users.EditUser
{
    internal static class Errors
    {
        public static Error Unauthorized() =>
            Error.Unauthorized(
                "EditUser.Unauthorized",
                "User is unauthorized.");

        public static Error UserNotFound() =>
            Error.NotFound(
                "EditUser.NotFound",
                "User not found.");

        public static Error UserWithIdNotFound(string userId) =>
            Error.NotFound(
                "EditUser.NotFound",
                $"User with ID {userId} not found.");

        public static Error UserHasDifferentLocation(string userId) =>
            Error.Conflict(
                "EditUser.Conflict",
                $"User with ID {userId} has a different location.");

        public static Error UserHasNoLocation() =>
            Error.NotFound(
                "EditUser.NotFound",
                "User has no location assigned.");
    }
}