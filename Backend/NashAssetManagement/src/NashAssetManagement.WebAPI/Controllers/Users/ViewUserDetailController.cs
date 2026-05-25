using Microsoft.AspNetCore.Mvc;
using Asp.Versioning;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using GetUserDetail = NashAssetManagement.Application.UseCases.Users.ViewDetail;
using NashAssetManagement.WebAPI.Utilities;
using NashAssetManagement.Domain.Constants;


namespace NashAssetManagement.WebAPI.Controllers.Users
{
    [ApiVersion(1)]
    [Route("api/v{version:apiVersion}/users/{id:guid}")]
    public class ViewUserDetailController(
        ISender sender,
        ILogger<ViewUserDetailController> logger) 
        : BaseApiController(sender)
    {
        [HttpGet]
        // [Authorize(Roles = ApplicationRole.Admin)]
        [ProducesResponseType(typeof(GetUserDetail.Response), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status403Forbidden)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetDetail(Guid id)
        {
            logger.LogInformation("Getting user detail with id: {id}", id);
            var result = await _sender.Send(new GetUserDetail.Query { Id = id });

            return result.Match(
                onValue: data =>
                {
                    logger.LogInformation("Successfully retrieved user detail for ID: {id}", id);
                    return Ok(data);
                },
                onError: errors =>
                {
                    logger.LogWarning("Failed to get user detail for ID: {id}, Errors: {@Errors}", id, errors);
                    return errors.ToProblem();
                }
            );
        }
    }
}