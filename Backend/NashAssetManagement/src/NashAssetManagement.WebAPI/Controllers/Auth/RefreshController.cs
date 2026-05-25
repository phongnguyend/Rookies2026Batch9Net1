using Asp.Versioning;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using NashAssetManagement.Application.Abstractions.Cookie;
using NashAssetManagement.Application.Abstractions.DateTimes;
using NashAssetManagement.Application.UseCases.Auth.Refresh;
using NashAssetManagement.Infrastructure.Jwt;
using NashAssetManagement.WebAPI.Utilities;

namespace NashAssetManagement.WebAPI.Controllers.Auth
{
    [ApiVersionNeutral]
    [Route("api/auth/refresh")]
    public sealed class RefreshController(
        ISender sender,
        ICookieService cookieService,
        IDateTimeProvider dateTimeProvider,
        IOptions<JwtOptions> jwtOptions)
        : BaseApiController(sender)
    {
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Refresh(CancellationToken cancellationToken)
        {
            // Retrieve refresh token from cookie
            Request.Cookies.TryGetValue(Domain.Constants.JwtTokenConstants.CookieRefreshToken, out var refreshToken);

            var request = new Request(refreshToken ?? string.Empty);
            var result = await _sender.Send(request, cancellationToken);
            if (result.IsError)
            {
                // Delete old cookies
                Response.Cookies.Delete(Domain.Constants.JwtTokenConstants.CookieAccessToken);
                Response.Cookies.Delete(Domain.Constants.JwtTokenConstants.CookieRefreshToken);

                return result.Errors.ToProblem();
            }

            // Set access token to cookie
            var accessTokenCookie = cookieService.CreateAccessTokenCookie(
                result.Value.AccessToken,
                dateTimeProvider.UtcNow.AddMilliseconds(jwtOptions.Value.AccessTokenExpiryInMilliseconds));
            Response.AppendAuthCookie(accessTokenCookie);

            // Set refresh token to cookie
            var refreshTokenCookie = cookieService.CreateRefreshTokenCookie(
                result.Value.RefreshToken,
                dateTimeProvider.UtcNow.AddMilliseconds(jwtOptions.Value.RefreshTokenExpiryInMilliseconds));
            Response.AppendAuthCookie(refreshTokenCookie);

            return Ok();
        }
    }
}