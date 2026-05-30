using Asp.Versioning;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NashAssetManagement.Application.UseCases.Assignments.UserAcceptingAssignment;
using NashAssetManagement.WebAPI.Utilities;
using Swashbuckle.AspNetCore.Annotations;

namespace NashAssetManagement.WebAPI.Controllers.Assignments
{
    [ApiVersion(1)]
    [Route("api/v{version:apiVersion}/user/assignments")]
    public class UserAcceptingAssignmentController
        : BaseApiController
    {
        public UserAcceptingAssignmentController(ISender sender) : base(sender)
        {
        }

        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status403Forbidden)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [Authorize]
        [HttpPut("{assignmentId}/accept")]
        [SwaggerOperation(
            Summary = "Accept asset assigned to user.",
            Description = "Allow a user to accept an assignment that has been assigned to them. Assignment need to be assigned to user and in 'Waiting for acceptance' state. Required user to be authenticated.",
            Tags = [ControllerTags.Assignments])]
        public async Task<IActionResult> UserAcceptingAssignment(
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
