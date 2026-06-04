using Asp.Versioning;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NashAssetManagement.Application.UseCases.Report.Cancel;
using NashAssetManagement.Domain.Constants;
using NashAssetManagement.WebAPI.Utilities;
using Swashbuckle.AspNetCore.Annotations;

namespace NashAssetManagement.WebAPI.Controllers.Reports
{
    [ApiVersion(1)]
    [Authorize(Roles = ApplicationRole.Admin)]
    [Route("api/v{version:apiVersion}/report/export")]
    public class ReportCancelController(ISender sender) : BaseApiController(sender)
    {
        [HttpDelete]
        [ProducesResponseType(typeof(Response), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status403Forbidden)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        [SwaggerOperation(
            Summary = "Cancel/delete current report.",
            Description = "Deletes the current administrator's export report job and deletes the generated Excel file from server storage.",
            Tags = [ControllerTags.Reports])]
        public async Task<IActionResult> Cancel(CancellationToken cancellationToken = default)
        {
            var result = await _sender.Send(new Request(), cancellationToken);

            return result.Match(
                Ok,
                errors => errors.ToProblem()
            );
        }
    }
}
