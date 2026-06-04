using Asp.Versioning;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NashAssetManagement.Application.UseCases.Report.CurrentDownload;
using NashAssetManagement.Domain.Constants;
using NashAssetManagement.WebAPI.Utilities;
using Swashbuckle.AspNetCore.Annotations;

namespace NashAssetManagement.WebAPI.Controllers.Reports
{
    [ApiVersion(1)]
    [Authorize(Roles = ApplicationRole.Admin)]
    [Route("api/v{version:apiVersion}/report/export")]
    public class ReportCurrentDownloadController(ISender sender) : BaseApiController(sender)
    {
        [HttpGet]
        [ProducesResponseType(typeof(Response), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status403Forbidden)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        [SwaggerOperation(
            Summary = "Get current export report status.",
            Description = "Retrieves the current export report status of the authenticated administrator.",
            Tags = [ControllerTags.Reports])]
        public async Task<IActionResult> GetCurrentReport(
            CancellationToken cancellationToken = default)
        {
            var result = await _sender.Send(
                new Request(),
                cancellationToken);

            return result.Match(
                Ok,
                errors => errors.ToProblem());
        }
    }
}