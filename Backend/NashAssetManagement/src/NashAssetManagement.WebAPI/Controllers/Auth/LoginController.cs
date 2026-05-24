using Asp.Versioning;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using NashAssetManagement.Application.Abstractions.Cookie;
using NashAssetManagement.Application.Abstractions.DateTimes;
using NashAssetManagement.Application.UseCases.Auth.Login;
using NashAssetManagement.Infrastructure.Jwt;
using NashAssetManagement.WebAPI.Utilities;

namespace NashAssetManagement.WebAPI.Controllers.Auth
{
    [ApiVersion(1)]
    [Route("api/v{version:apiVersion}/auth/login")]
    public sealed class AuthController(
        ISender sender,
        ICookieService cookieService,
        IDateTimeProvider dateTimeProvider,
        IOptions<JwtOptions> jwtOptions)
        : BaseApiController(sender)
    {
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Login(
            [FromBody] Request request,
            CancellationToken cancellationToken)
        {
            var result = await _sender.Send(request, cancellationToken);
            if (result.IsError)
            {
                return result.Errors.ToProblem();
            }

            // Set access token to cookie
            var accessTokenCookie = cookieService.CreateAccessTokenCookie(
                result.Value.AccessToken,
                dateTimeProvider.UtcNow.AddMilliseconds(jwtOptions.Value.AccessTokenExpiryInMilliseconds));

            Response.Cookies.Append(
                accessTokenCookie.TokenName,
                accessTokenCookie.Value,
                new CookieOptions
                {
                    HttpOnly = accessTokenCookie.HttpOnly,
                    Secure = accessTokenCookie.Secure,
                    IsEssential = accessTokenCookie.IsEssential,
                    Path = accessTokenCookie.Path,
                    Expires = accessTokenCookie.ExpiresAtUtc,
                    SameSite = SameSiteMode.Lax // set here due to http at the webapi, not application level
                });

            // Set refresh token to cookie
            var refreshTokenCookie = cookieService.CreateRefreshTokenCookie(
                result.Value.RefreshToken,
                dateTimeProvider.UtcNow.AddMilliseconds(jwtOptions.Value.RefreshTokenExpiryInMilliseconds));

            Response.Cookies.Append(
                refreshTokenCookie.TokenName,
                refreshTokenCookie.Value,
                new CookieOptions
                {
                    HttpOnly = refreshTokenCookie.HttpOnly,
                    Secure = refreshTokenCookie.Secure,
                    IsEssential = refreshTokenCookie.IsEssential,
                    Path = refreshTokenCookie.Path,
                    Expires = refreshTokenCookie.ExpiresAtUtc,
                    SameSite = SameSiteMode.Lax
                });

            return Ok();
        }
    }
}