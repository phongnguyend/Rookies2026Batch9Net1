using Asp.Versioning;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NashAssetManagement.Application.UseCases.Auth.Logout;
using NashAssetManagement.Domain.Constants;
using NashAssetManagement.WebAPI.Utilities;

namespace NashAssetManagement.WebAPI.Controllers.Auth
{
    [ApiVersion(1)]
    [Route("api/v{version:apiVersion}/auth/logout")]
    public sealed class LogoutController(ISender sender) : BaseApiController(sender)
    {
        [Authorize]
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        public async Task<IActionResult> Logout(CancellationToken cancellationToken)
        {
            var result = await _sender.Send(new Request(), cancellationToken);

            Response.Cookies.Delete(JwtTokenConstants.CookieAccessToken);
            Response.Cookies.Delete(JwtTokenConstants.CookieRefreshToken);

            if (result.IsError)
            {
                return result.Errors.ToProblem();
            }

            return NoContent();
        }
    }
}