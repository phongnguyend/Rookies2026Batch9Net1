using ErrorOr;

namespace NashAssetManagement.Application.UseCases.Report.Cancel
{
    internal static class Errors
    {
        public static readonly Error UnauthorizedUser = Error.Unauthorized(
            code: "ReportCancel.UnauthorizedUser",
            description: "User is not authorized to do this action.");

        public static readonly Error UnidentifiedUser = Error.Unauthorized(
            code: "ReportCancel.UnidentifiedUser",
            description: "Cannot identify the current user.");

        public static readonly Error ReportNotFound = Error.NotFound(
            code: "ReportCancel.ReportNotFound",
            description: "No active report found to cancel.");
    }
}
