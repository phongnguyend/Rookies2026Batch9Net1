using Asp.Versioning;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NashAssetManagement.Application.UseCases.Assignments.GetEditingAssignment;
using NashAssetManagement.Domain.Constants;
using NashAssetManagement.WebAPI.Utilities;
using Swashbuckle.AspNetCore.Annotations;

namespace NashAssetManagement.WebAPI.Controllers.Assignments
{
    [ApiVersion(1)]
    [Route("api/v{version:apiVersion}/assignments")]
    public class GetEditingAssignmentController
        : BaseApiController
    {
        public GetEditingAssignmentController(ISender sender) : base(sender)
        {
        }

        [ProducesResponseType(typeof(Response), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status403Forbidden)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        [Authorize(Roles = ApplicationRole.Admin)]
        [HttpGet("{assignmentId}/edit")]
        [SwaggerOperation(
            Summary = "Get assignment's detail for editing.",
            Description = "Allow admin to get necessary information of assignment for editing. Required admin to be authenticated.",
            Tags = [ControllerTags.Assignments])]
        public async Task<IActionResult> GetEditingAssignment(
            [FromRoute] string? assignmentId,
            CancellationToken cancellationToken)
        {
            var request = new Request(assignmentId);
            var result = await _sender.Send(request, cancellationToken);
            return result.Match(
                Ok,
                errors => errors.ToProblem());
        }
    }
}
