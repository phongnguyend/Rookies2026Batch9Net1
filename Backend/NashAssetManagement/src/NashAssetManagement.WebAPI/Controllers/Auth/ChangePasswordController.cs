using Asp.Versioning;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using NashAssetManagement.Application.UseCases.Auth.ChangePassword;
using NashAssetManagement.Infrastructure.Jwt;
using NashAssetManagement.WebAPI.Utilities;
using Swashbuckle.AspNetCore.Annotations;

namespace NashAssetManagement.WebAPI.Controllers.Auth
{
    [ApiVersion(1)]
    [Route("api/v{version:apiVersion}/auth/change-password")]
    public sealed class ChangePasswordController(
        ISender sender
    )
        : BaseApiController(sender)
    {
        [Authorize]
        [HttpPost]
        [SwaggerOperation(
            Summary = "Change password for the currently authenticated user.",
            Description = "Allows an authenticated user to change their password by providing the current password and a new password.")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> ChangePassword(
            [FromBody] Request command,
            CancellationToken cancellationToken)
        {
            var result = await _sender.Send(command, cancellationToken);
            
            return result.Match(
                _ => NoContent(),
                errors => errors.ToProblem());
        }
    }
}