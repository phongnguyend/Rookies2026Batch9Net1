using ErrorOr;

namespace NashAssetManagement.Application.UseCases.Users.ViewUserForEditing
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

        public static Error UserWithIdNotFound(string userId) =>
            Error.NotFound(
                "Users.NotFound",
                $"User with ID {userId} not found.");

        public static Error UserHasDifferentLocation() =>
            Error.Conflict(
                "Users.Conflict",
                "You are not allowed to edit the information of users in a different location.");

        public static Error UserHasNoLocation() =>
            Error.NotFound(
                "Users.NotFound",
                "User has no location assigned.");
    }
}
