using Asp.Versioning;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NashAssetManagement.Application.UseCases.ReturnRequests.UserCreateReturnRequest;
using NashAssetManagement.WebAPI.Utilities;
using Swashbuckle.AspNetCore.Annotations;

namespace NashAssetManagement.WebAPI.Controllers.ReturnRequests
{
    [ApiVersion(1)]
    [Route("api/v{version:apiVersion}/user/assignments")]
    public class UserCreateReturnRequestController(
        ISender sender)
        : BaseApiController(sender)
    {
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status403Forbidden)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status409Conflict)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [Authorize]
        [SwaggerOperation(
            Summary = "Allow user to create return request for their assignment.",
            Tags = [ControllerTags.ReturningRequests])]
        [HttpPost("{assignmentId}/return-request")]
        public async Task<IActionResult> UserCreateReturnRequest(
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
