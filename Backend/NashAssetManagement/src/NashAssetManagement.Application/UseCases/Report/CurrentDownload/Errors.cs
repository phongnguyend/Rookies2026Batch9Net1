using ErrorOr;

namespace NashAssetManagement.Application.UseCases.Report.CurrentDownload
{
    internal static class Errors
    {
        public static readonly Error UnauthorizedUser = Error.Unauthorized(
            code: "ReportCurrentDownload.UnauthorizedUser",
            description: "User is not authorized to do this action.");

        public static readonly Error UnidentifiedUser = Error.Unauthorized(
            code: "ReportCurrentDownload.UnidentifiedUser",
            description: "Cannot identify the current user.");
    }
}