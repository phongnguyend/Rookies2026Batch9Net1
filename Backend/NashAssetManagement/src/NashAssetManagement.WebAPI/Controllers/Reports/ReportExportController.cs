using Asp.Versioning;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NashAssetManagement.Application.UseCases.Report.Export;
using NashAssetManagement.Domain.Constants;
using NashAssetManagement.WebAPI.Utilities;
using Swashbuckle.AspNetCore.Annotations;

namespace NashAssetManagement.WebAPI.Controllers.Reports
{
    [ApiVersion(1)]
    [Authorize(Roles = ApplicationRole.Admin)]
    [Route("api/v{version:apiVersion}/report/export")]
    public class ReportExportController(ISender sender) : BaseApiController(sender)
    {
        [HttpPost]
        [ProducesResponseType(typeof(Response), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status403Forbidden)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status409Conflict)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        [SwaggerOperation(
            Summary = "Start export report generation.",
            Description = "Creates an export report job for the current administrator.",
            Tags = [ControllerTags.Reports])]
        public async Task<IActionResult> Export([FromQuery] Request request, CancellationToken cancellationToken = default)
        {
            var result = await _sender.Send(request, cancellationToken);

            return result.Match(
                Ok,
                errors => errors.ToProblem()
            );
        }
    }
}