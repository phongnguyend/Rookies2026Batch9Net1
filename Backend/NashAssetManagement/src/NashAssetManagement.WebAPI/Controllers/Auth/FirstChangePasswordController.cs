using Asp.Versioning;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NashAssetManagement.Application.UseCases.Auth.FirstChangePassword;
using NashAssetManagement.WebAPI.Utilities;

namespace NashAssetManagement.WebAPI.Controllers.Auth
{
    [Authorize]
    [ApiVersion(1)]
    [Route("api/v{version:apiVersion}/auth/first-change-password")]
    public sealed class FirstChangePasswordController(ISender sender) : BaseApiController(sender)
    {
        [HttpPost]
        [ProducesResponseType(typeof(Response), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status403Forbidden)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status409Conflict)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> FirstChangePassword(
            [FromBody] Request request,
            CancellationToken cancellationToken)
        {
            var result = await _sender.Send(request, cancellationToken);

            return result.Match(
                Ok,
                errors => errors.ToProblem());
        }
    }
}
