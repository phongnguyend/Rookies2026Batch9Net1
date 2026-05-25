using Microsoft.AspNetCore.Mvc;
using Asp.Versioning;
using MediatR;
using NashAssetManagement.Application.Utilities;
using GetUsers = NashAssetManagement.Application.UseCases.Users.ViewList;
using GetUserDetail = NashAssetManagement.Application.UseCases.Users.ViewDetail;
using NashAssetManagement.WebAPI.Utilities;
using Microsoft.AspNetCore.Authorization;

namespace NashAssetManagement.WebAPI.Controllers
{
    [ApiVersion(1)]
    [Route("api/v{version:apiVersion}/users")]
    public class UsersController : BaseApiController
    {
        private readonly ILogger<UsersController> _logger;
        
        public UsersController(
            ISender sender,
            ILogger<UsersController> logger) : base(sender)
        {
            _logger = logger;
        }

        [HttpGet]
        // [Authorize(Roles = "Admin")]
        [ProducesResponseType(typeof(PagedList<GetUsers.Response>), StatusCodes.Status200OK)]
        public async Task<IActionResult> Get([FromQuery] GetUsers.Query query)
        {
            _logger.LogInformation("Getting users with query: {@Query}", query);
            var result = await _sender.Send(query);

            return result.Match(
                onValue: data =>
                {
                    _logger.LogInformation("Successfully retrieved {TotalItems} users", data.TotalCount);
                    return Ok(data);    
                },
                onError: errors =>
                {
                    _logger.LogWarning("Failed to get users: {@Errors}", errors);
                    return errors.ToProblem();
                }
            );
        }

        [HttpGet("{id:guid}")]
        // [Authorize(Roles = "Admin")]
        [ProducesResponseType(typeof(GetUserDetail.Response), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetDetail(Guid id)
        {
            _logger.LogInformation("Getting user detail with id: {id}", id);
            var result = await _sender.Send(new GetUserDetail.Query { Id = id });

            return result.Match(
                onValue: data =>
                {
                    _logger.LogInformation("Successfully retrieved user detail for ID: {id}", id);
                    return Ok(data);
                },
                onError: errors =>
                {
                    _logger.LogWarning("Failed to get user detail for ID: {id}, Errors: {@Errors}", id, errors);
                    return errors.ToProblem();
                }
            );
        }
    }
}