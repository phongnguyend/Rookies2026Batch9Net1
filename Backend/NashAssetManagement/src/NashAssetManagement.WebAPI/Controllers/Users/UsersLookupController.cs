using Asp.Versioning;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NashAssetManagement.Application.UseCases.Users.UsersLookup;
using NashAssetManagement.Application.Utilities;
using NashAssetManagement.Domain.Constants;
using NashAssetManagement.WebAPI.Utilities;
using Swashbuckle.AspNetCore.Annotations;

namespace NashAssetManagement.WebAPI.Controllers.Users
{
    [ApiVersion(1)]
    [Route("api/v{version:apiVersion}/users")]
    public class UsersLookupController
        : BaseApiController
    {
        public UsersLookupController(ISender sender) : base(sender)
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
            Summary = "Lookup users to assign asset to.",
            Description = "Allow admin to lookup users in the system to assign asset. Users will be in the same location of admin. Required admin to be authenticated.",
            Tags = [ControllerTags.Users])]
        public async Task<IActionResult> LookupUser(
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
