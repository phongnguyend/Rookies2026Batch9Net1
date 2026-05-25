using Asp.Versioning;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NashAssetManagement.Application.UseCases.Assignments.ViewUserAssignments;
using NashAssetManagement.Application.Utilities;
using NashAssetManagement.WebAPI.Utilities;
using Swashbuckle.AspNetCore.Annotations;

namespace NashAssetManagement.WebAPI.Controllers.Assignments
{
    [ApiVersion(1)]
    [Route("api/v{version:apiVersion}/user/assignments")]
    public class ViewUserAssignmentsController
        : BaseApiController
    {
        public ViewUserAssignmentsController(ISender sender) : base(sender)
        {
        }

        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(typeof(PagedList<Response>), StatusCodes.Status200OK)]
        [Authorize]
        [HttpGet]
        [SwaggerOperation(
            Summary = "Get user's assignments. Assignments will be in state 'Waiting for acceptance' or 'Accepted'. Only assignments from the past to current date will be fetched.")]
        public async Task<IActionResult> ViewUserAssignments(
            [FromQuery] Request request,
            CancellationToken cancellationToken)
        {
            var result = await _sender.Send(request, cancellationToken);
            return result.Match
            (
                Ok,
                errors => ErrorOrExtensions.ToProblem(errors));
        }
    }
}
