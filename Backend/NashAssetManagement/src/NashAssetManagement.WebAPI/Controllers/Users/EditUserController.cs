using Asp.Versioning;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NashAssetManagement.Application.UseCases.Users.EditUser;
using NashAssetManagement.Domain.Constants;
using NashAssetManagement.WebAPI.Utilities;
using Swashbuckle.AspNetCore.Annotations;

namespace NashAssetManagement.WebAPI.Controllers.Users
{
    [ApiVersion(1)]
    [Route("api/v{version:apiVersion}/users")]
    public class EditUserController(
        ISender sender
    ) : BaseApiController(sender)
    {
        [HttpPut("{id}/edit")]
        [Authorize(Roles = ApplicationRole.Admin)]
        [ProducesResponseType(typeof(Response), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status403Forbidden)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status409Conflict)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        [SwaggerOperation(
            Tags = [ControllerTags.Users],
            Summary = "Allow admin to edit user information."
        )]
        public async Task<IActionResult> EditUser([FromBody] Request request, string id)
        {
            var result = await _sender.Send(request with
            {
                UserId = id
            });

            return result.Match(
                Ok,
                errors => ErrorOrExtensions.ToProblem(errors)
            );
        }
    }
}
