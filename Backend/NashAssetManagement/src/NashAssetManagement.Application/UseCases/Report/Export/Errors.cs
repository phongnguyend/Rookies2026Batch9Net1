using ErrorOr;

namespace NashAssetManagement.Application.UseCases.Report.Export
{
    internal static class Errors
    {
        public static Error UnauthorizedUser = Error.Unauthorized(
            "ReportExport.UnauthorizedUser",
            "User is not authorized to do this action.");

        public static Error UnidentifiedUser = Error.Unauthorized(
            "ReportExport.UnidentifiedUser",
            "Cannot identify user with user's ID.");

        public static readonly Error ReportAlreadyExists = Error.Conflict(
            code: "ReportExport.ReportAlreadyExists",
            description: "A report is already being processed or waiting for download.");

        public static readonly Error ReportCreationFailed = Error.Failure(
            code: "ReportExport.ReportCreationFailed",
            description: "Failed to create export report. Please try again");
    }
}
