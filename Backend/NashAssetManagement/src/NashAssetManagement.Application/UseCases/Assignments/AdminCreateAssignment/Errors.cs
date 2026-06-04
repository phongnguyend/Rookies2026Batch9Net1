using ErrorOr;

namespace NashAssetManagement.Application.UseCases.Assignments.AdminCreateAssignment
{
    internal static class Errors
    {
        public static Error UnauthorizedUser =
            Error.Unauthorized(
                "AdminCreateAssignment.UnauthorizedUser",
                "User is not authorized to do this action.");

        public static Error UnidentifiedUser =
            Error.Unauthorized(
                "AdminCreateAssignment.UnidentifiedUser",
                "Cannot identify current user with user's ID.");

        public static readonly Error StaffNotFound =
           Error.NotFound(
               "AdminCreateAssignment.StaffNotFound",
               "Staff profile was not found.");

        public static readonly Error StaffNotInSameLocation =
           Error.NotFound(
               "AdminCreateAssignment.StaffNotInSameLocation",
               "Staff is not in the same location.");

        public static readonly Error AssetNotFound =
          Error.NotFound(
              "AdminCreateAssignment.AssetNotFound",
              "Asset was not found.");

        public static readonly Error AssetNotInSameLocation =
           Error.NotFound(
               "AdminCreateAssignment.AssetNotInSameLocation",
               "Asset is not in the same location.");

        public static readonly Error AssetNotAvailable =
           Error.Conflict(
               "AdminCreateAssignment.AssetNotAvailable",
               "Only available assets can be assigned.");

        public static Error UnexpectedErrorOccurred =
            Error.Failure(
                "AdminCreateAssignment.UnexpectedErrorOccurred",
                "Unexpected error occurred when trying to create assignment. Please try again.");

    }
}
