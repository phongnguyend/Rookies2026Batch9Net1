using Asp.Versioning;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NashAssetManagement.Application.UseCases.ReturnRequests.AdminCompleteReturnRequest;
using NashAssetManagement.Domain.Constants;
using NashAssetManagement.WebAPI.Utilities;
using Swashbuckle.AspNetCore.Annotations;

namespace NashAssetManagement.WebAPI.Controllers.ReturnRequests
{
    [Authorize(Roles = ApplicationRole.Admin)]
    [ApiVersion(1)]
    [Route("api/v{version:apiVersion}/admin/return-requests")]
    public class AdminCompleteReturnRequestController(
        ISender sender)
        : BaseApiController(sender)
    {
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status403Forbidden)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status409Conflict)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [SwaggerOperation(
            Summary = "Complete a return request",
            Description = "Allows an admin to mark a return request as completed.",
            Tags = [ControllerTags.ReturningRequests])]
        [HttpPost("{returnRequestId}/complete")]
        public async Task<IActionResult> AdminCompleteReturnRequest(
            [FromRoute] string? returnRequestId,
            CancellationToken cancellationToken)
        {
            var request = new Request(returnRequestId);

            var result = await _sender.Send(
                request,
                cancellationToken);

            return result.IsError
                ? result.Errors.ToProblem()
                : NoContent();
        }
    }
}
