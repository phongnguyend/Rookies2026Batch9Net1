using Asp.Versioning;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NashAssetManagement.Application.UseCases.Users.Disable;
using NashAssetManagement.Domain.Constants;
using NashAssetManagement.WebAPI.Utilities;
using Swashbuckle.AspNetCore.Annotations;

namespace NashAssetManagement.WebAPI.Controllers.Users
{
    [ApiVersion(1)]
    [Route("api/v{version:apiVersion}/users")]
    public sealed class DisableUserController(
        ISender sender)
        : BaseApiController(sender)
    {
        [HttpPatch("{id}/disable")]
        [Authorize(Roles = ApplicationRole.Admin)]
        [ProducesResponseType(typeof(Response), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status403Forbidden)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        [SwaggerOperation(
            Tags = [ControllerTags.Users],
            Summary = "Disable (soft-delete) a target user."
        )]
        public async Task<IActionResult> DisableUser(string id)
        {
            var result = await _sender.Send(new Request(id));

            return result.Match(
                response => Ok(response),
                errors => errors.ToProblem()
            );
        }
    }
}
