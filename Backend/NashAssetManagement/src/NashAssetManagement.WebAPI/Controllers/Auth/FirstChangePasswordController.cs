using Asp.Versioning;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using NashAssetManagement.Application.Abstractions.Cookie;
using NashAssetManagement.Application.Abstractions.DateTimes;
using NashAssetManagement.Application.UseCases.Auth.FirstChangePassword;
using NashAssetManagement.Infrastructure.Jwt;
using NashAssetManagement.WebAPI.Utilities;

namespace NashAssetManagement.WebAPI.Controllers.Auth
{
    [Authorize]
    [ApiVersion(1)]
    [Route("api/v{version:apiVersion}/auth/first-change-password")]
    public sealed class FirstChangePasswordController(
        ISender sender,
        ICookieService cookieService,
        IDateTimeProvider dateTimeProvider,
        IOptions<JwtOptions> jwtOptions)
        : BaseApiController(sender)
    {
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status200OK)]
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
            if (result.IsError)
            {
                return result.Errors.ToProblem();
            }

            // Set new access token to cookie
            var accessTokenCookie = cookieService.CreateAccessTokenCookie(
                result.Value.AccessToken,
                dateTimeProvider.UtcNow.AddMilliseconds(jwtOptions.Value.AccessTokenExpiryInMilliseconds));
            Response.AppendAuthCookie(accessTokenCookie);

            // Set new refresh token to cookie
            var refreshTokenCookie = cookieService.CreateRefreshTokenCookie(
                result.Value.RefreshToken,
                dateTimeProvider.UtcNow.AddMilliseconds(jwtOptions.Value.RefreshTokenExpiryInMilliseconds));
            Response.AppendAuthCookie(refreshTokenCookie);

            return Ok();
        }
    }
}
