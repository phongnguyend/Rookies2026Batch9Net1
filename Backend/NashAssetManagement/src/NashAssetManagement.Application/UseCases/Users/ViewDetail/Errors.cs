using ErrorOr;

namespace NashAssetManagement.Application.UseCases.Users.ViewDetail
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

        public static Error UserWithIdNotFound(Guid userId) =>
            Error.NotFound(
                "Users.NotFound",
                $"User with ID {userId} not found.");
    }
}