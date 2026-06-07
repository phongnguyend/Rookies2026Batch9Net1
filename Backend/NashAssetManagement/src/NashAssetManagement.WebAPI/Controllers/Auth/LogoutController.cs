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
        // allow invalid, expired cookie to be deleted, rather than only [Authorize] cookie
        [AllowAnonymous]
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        public async Task<IActionResult> Logout(CancellationToken cancellationToken)
        {
            var result = await _sender.Send(new Request(), cancellationToken);

            Response.DeleteAcccessCookie(JwtTokenConstants.CookieAccessToken);
            Response.DeleteRefreshCookie(JwtTokenConstants.CookieRefreshToken);

            if (result.IsError)
            {
                return result.Errors.ToProblem();
            }

            return NoContent();
        }
    }
}