using Asp.Versioning;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NashAssetManagement.Application.Utilities;
using NashAssetManagement.WebAPI.Utilities;
using GetUsers = NashAssetManagement.Application.UseCases.Users.ViewList;
using NashAssetManagement.Domain.Constants;

namespace NashAssetManagement.WebAPI.Controllers.Users
{
    [ApiVersion(1)]
    [Route("api/v{version:apiVersion}/users")]
    public sealed class ViewUserListController(
        ISender sender,
        ILogger<ViewUserListController> logger) 
    : BaseApiController(sender)
    {
        [HttpGet]
        [Authorize(Roles = ApplicationRole.Admin)]
        [ProducesResponseType(typeof(PagedList<GetUsers.Response>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status403Forbidden)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Get([FromQuery] GetUsers.Query query)
        {
            logger.LogInformation("Getting users with query: {@Query}", query);
            var result = await _sender.Send(query);

            return result.Match(
                onValue: data =>
                {
                    logger.LogInformation("Successfully retrieved {TotalItems} users", data.TotalCount);
                    return Ok(data);    
                },
                onError: errors =>
                {
                    logger.LogWarning("Failed to get users: {@Errors}", errors);
                    return errors.ToProblem();
                }
            );
        }
    }
}