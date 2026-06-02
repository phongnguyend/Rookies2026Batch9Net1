using Asp.Versioning;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NashAssetManagement.Application.UseCases.Assets.AssetsLookup;
using NashAssetManagement.Application.Utilities;
using NashAssetManagement.Domain.Constants;
using NashAssetManagement.WebAPI.Utilities;
using Swashbuckle.AspNetCore.Annotations;

namespace NashAssetManagement.WebAPI.Controllers.Assets
{
    [ApiVersion(1)]
    [Route("api/v{version:apiVersion}/assets")]
    public class AssetsLookupController
        : BaseApiController
    {
        public AssetsLookupController(ISender sender) : base(sender)
        {
        }

        [HttpGet("lookup")]
        [Authorize(Roles = ApplicationRole.Admin)]
        [ProducesResponseType(typeof(PagedList<Response>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status403Forbidden)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        [SwaggerOperation(
            Summary = "Lookup assets to assign to user.",
            Description = "Allow admin to lookup assets that available for assigning. Required admin to be authenticated.",
            Tags = [ControllerTags.Assets])]
        public async Task<IActionResult> AssetsLookup(
            [FromQuery] Request request,
            CancellationToken cancellationToken)
        {
            var result = await _sender.Send(request, cancellationToken);
            return result.Match(
                Ok,
                errors => errors.ToProblem());
        }
    }
}
