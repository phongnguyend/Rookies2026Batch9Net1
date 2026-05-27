using Asp.Versioning;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NashAssetManagement.Application.UseCases.Users.ViewUsers;
using NashAssetManagement.Application.Utilities;
using NashAssetManagement.Domain.Constants;
using NashAssetManagement.WebAPI.Utilities;
using Swashbuckle.AspNetCore.Annotations;

namespace NashAssetManagement.WebAPI.Controllers.Users
{
    [ApiVersion(1)]
    [Route("api/v{version:apiVersion}/users")]
    public sealed class ViewUserListController(
        ISender sender) 
    : BaseApiController(sender)
    {
        [HttpGet]
        [Authorize(Roles = ApplicationRole.Admin)]
        [ProducesResponseType(typeof(PagedList<Response>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status403Forbidden)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        [SwaggerOperation(Tags = [ControllerTags.Users])]
        public async Task<IActionResult> Get([FromQuery] Request request)
        {
            var result = await _sender.Send(request);

            return result.Match(
                Ok,
                errors => ErrorOrExtensions.ToProblem(errors)
            );
        }
    }
}