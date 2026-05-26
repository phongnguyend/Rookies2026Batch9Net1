using ErrorOr;

namespace NashAssetManagement.Application.UseCases.Assignments.GetAll
{
    public static class Errors
    {
        public static readonly Error UserNotFound = Error.NotFound(
            code: "Profile.UserNotFound",
            description: "User profile was not found.");
    }
}
