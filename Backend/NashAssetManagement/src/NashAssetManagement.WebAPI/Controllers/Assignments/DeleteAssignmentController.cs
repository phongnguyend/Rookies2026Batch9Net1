using Asp.Versioning;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NashAssetManagement.Application.UseCases.Assignments.DeleteAssignment;
using NashAssetManagement.Domain.Constants;
using NashAssetManagement.WebAPI.Utilities;
using Swashbuckle.AspNetCore.Annotations;

namespace NashAssetManagement.WebAPI.Controllers.Assignments
{
    [ApiVersion(1)]
    [Route("api/v{version:apiVersion}/admin/assignments")]
    public class DeleteAssignmentController
        : BaseApiController
    {
        public DeleteAssignmentController(ISender sender) : base(sender)
        {
        }

        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status403Forbidden)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [Authorize(Roles = ApplicationRole.Admin)]
        [HttpDelete("{assignmentId}")]
        [SwaggerOperation(
            Summary = "Delete assignment.",
            Description = "Allow an admin to delete an assignment. Required user to be authenticated and have admin role.",
            Tags = [ControllerTags.Assignments])]
        public async Task<IActionResult> DeleteAssignment(
            [FromRoute] string? assignmentId,
            CancellationToken cancellationToken)
        {
            var request = new Request(assignmentId);
            var result = await _sender.Send(request, cancellationToken);
            return result.IsError
                ? result.Errors.ToProblem()
                : NoContent();
        }
    }
}