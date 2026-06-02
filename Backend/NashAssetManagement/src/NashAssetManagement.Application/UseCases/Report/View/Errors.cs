using ErrorOr;

namespace NashAssetManagement.Application.UseCases.Report.View
{
    internal static class Errors
    {
        public static Error UnauthorizedUser = Error.Unauthorized(
            "ReportView.UnauthorizedUser",
            "User is not authorized to do this action.");

        public static Error UnidentifiedUser = Error.Unauthorized(
            "ReportView.UnidentifiedUser",
            "Cannot identify user with user's ID.");
    }
}
