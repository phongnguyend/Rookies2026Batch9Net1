using Asp.Versioning;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NashAssetManagement.Application.UseCases.ReturnRequests.ViewList;
using NashAssetManagement.Application.Utilities;
using NashAssetManagement.Domain.Constants;
using NashAssetManagement.WebAPI.Utilities;
using Swashbuckle.AspNetCore.Annotations;

namespace NashAssetManagement.WebAPI.Controllers.ReturnRequests
{
    [ApiVersion(1)]
    [Route("api/v{version:apiVersion}/return-requests")]
    public class ViewReturnRequestsController
        : BaseApiController
    {
        public ViewReturnRequestsController(ISender sender) : base(sender)
        {
        }

        [HttpGet]
        [Authorize(Roles = ApplicationRole.Admin)]
        [ProducesResponseType(typeof(PagedList<Response>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status403Forbidden)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        [SwaggerOperation(
            Tags = [ControllerTags.ReturningRequests],
            Summary = "Allow admin to view return requests."
        )]
        public async Task<IActionResult> ViewReturnRequests(
            [FromQuery] Request request,
            CancellationToken cancellationToken
        )
        {
            var result = await _sender.Send(request, cancellationToken);
            return result.Match(
                Ok,
                errors => ErrorOrExtensions.ToProblem(errors)
            );
        }
    }
}
