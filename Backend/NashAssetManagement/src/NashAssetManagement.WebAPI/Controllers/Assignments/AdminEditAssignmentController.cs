using Asp.Versioning;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NashAssetManagement.Application.UseCases.Assignments.AdminEditAssignment;
using NashAssetManagement.Domain.Constants;
using NashAssetManagement.WebAPI.Utilities;
using Swashbuckle.AspNetCore.Annotations;

namespace NashAssetManagement.WebAPI.Controllers.Assignments
{
    [ApiVersion(1)]
    [Route("api/v{version:apiVersion}/assignments")]
    public class AdminEditAssignmentController
        : BaseApiController
    {
        public AdminEditAssignmentController(ISender sender) : base(sender)
        {
        }

        [ProducesResponseType(typeof(Response), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status403Forbidden)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        [Authorize(Roles = ApplicationRole.Admin)]
        [HttpPatch("{assignmentId}/edit")]
        [SwaggerOperation(
            Summary = "Admin edit an assignment.",
            Description = "Allow admin to edit an assignment. Required admin to be authenticated.",
            Tags = [ControllerTags.Assignments])]
        public async Task<IActionResult> AdminEditAssignment(
            [FromRoute] string? assignmentId,
            [FromBody] AssignmentEditPayload? payload,
            CancellationToken cancellationToken)
        {
            var request = new Request(assignmentId, payload);
            var result = await _sender.Send(request, cancellationToken);
            return result.Match(
                Ok,
                errors => errors.ToProblem());
        }
    }
}
