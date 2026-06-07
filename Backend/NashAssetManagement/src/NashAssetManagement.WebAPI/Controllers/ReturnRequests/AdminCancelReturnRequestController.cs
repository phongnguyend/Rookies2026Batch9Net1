using Asp.Versioning;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NashAssetManagement.Application.UseCases.ReturnRequests.AdminCancelReturnRequest;
using NashAssetManagement.Domain.Constants;
using NashAssetManagement.WebAPI.Utilities;
using Swashbuckle.AspNetCore.Annotations;

namespace NashAssetManagement.WebAPI.Controllers.ReturnRequests
{
    [ApiVersion(1)]
    [Route("api/v{version:apiVersion}/admin/return-requests")]
    public class AdminCancelReturnRequestController
        : BaseApiController
    {
        public AdminCancelReturnRequestController(ISender sender) : base(sender)
        {
        }

        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status403Forbidden)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [Authorize(Roles = ApplicationRole.Admin)]
        [HttpPatch("{returnRequestId}/cancel")]
        [SwaggerOperation(
            Tags = [ControllerTags.ReturningRequests],
            Summary = "Admin cancel returning request.",
            Description = "Allow admin to cancel return request of an assignment/asset. Required admin to be authenticated."
        )]
        public async Task<IActionResult> AdminCancelReturnRequest(
            [FromRoute] string? returnRequestId,
            CancellationToken cancellationToken)
        {
            var request = new Request(returnRequestId);
            var result = await _sender.Send(request, cancellationToken);
            return result.Match(
                _ => NoContent(),
                errors => errors.ToProblem());
        }
    }
}
