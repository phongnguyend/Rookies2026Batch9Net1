using Asp.Versioning;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NashAssetManagement.Application.UseCases.Assignments.ViewUserAssignmentDetail;
using NashAssetManagement.WebAPI.Utilities;
using Swashbuckle.AspNetCore.Annotations;

namespace NashAssetManagement.WebAPI.Controllers.Assignments
{
    [ApiVersion(1)]
    [Route("api/v{version:apiVersion}/user/assignments")]
    public class ViewUserAssignmentDetailController
        : BaseApiController
    {
        public ViewUserAssignmentDetailController(ISender sender) : base(sender)
        {
        }

        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(typeof(Response), StatusCodes.Status200OK)]
        [Authorize]
        [HttpGet("{assignmentId}/detail")]
        [SwaggerOperation(
            Summary = "Get user assignment's detail.")]
        public async Task<IActionResult> ViewUserAssignmentDetail(
            [FromRoute] string? assignmentId,
            CancellationToken cancellationToken)
        {
            var request = new Request(assignmentId);
            var result = await _sender.Send(request, cancellationToken);
            return result.Match
            (
                Ok,
                errors => ErrorOrExtensions.ToProblem(errors));
        }
    }
}
