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

        public static Error AdminNotAllowedToEditOwnType() =>
            Error.Conflict(
                "EditUser.Conflict",
                "Admin is not allowed to edit their own user type.");

        public static Error UserWasModified() =>
            Error.Conflict(
                "EditUser.ConcurrencyConflict",
                "This user was updated by someone else. Please reload and try again.");

        public static Error LocationMustHaveAtLeastOneAdmin() =>
            Error.Conflict(
                "EditUser.LocationMustHaveAtLeastOneAdmin",
                "Location must have at least one admin.");
                
        public static Error InvalidUserType(string userType) =>
            Error.Validation(
                "EditUser.InvalidUserType",
                $"Invalid user type: {userType}");

        public static Error FailedToUpdateUserRole(string userId, string newType) =>
            Error.Failure(
                "EditUser.FailedToUpdateUserRole",
                $"Failed to update user role for user with ID {userId} to {newType}.");

        public static Error FailedToUpdateUser(string userId) =>
            Error.Failure(
                "EditUser.FailedToUpdateUser",
                $"Failed to update user with ID {userId}.");

        public static Error UnexpectedErrorOccurred() =>
            Error.Failure(
                "EditUser.UnexpectedErrorOccurred",
                "An unexpected error occurred while trying to edit user. Please try again.");
    }
}
