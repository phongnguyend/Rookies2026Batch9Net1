using Asp.Versioning;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NashAssetManagement.Application.UseCases.Auth.Profile;
using NashAssetManagement.WebAPI.Utilities;

namespace NashAssetManagement.WebAPI.Controllers.Auth
{
    [Authorize]
    [ApiVersion(1)]
    [Route("api/v{version:apiVersion}/auth/profile")]
    public sealed class ProfileController(ISender sender) : BaseApiController(sender)
    {
        [HttpGet]
        [ProducesResponseType(typeof(Response), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> GetProfile(CancellationToken cancellationToken)
        {
            var request = new Request();
            var result = await _sender.Send(request, cancellationToken);

            return result.Match(
                Ok,
                errors => errors.ToProblem());
        }
    }
}
