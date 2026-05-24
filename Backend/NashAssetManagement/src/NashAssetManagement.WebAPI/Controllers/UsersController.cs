using Microsoft.AspNetCore.Mvc;
using Asp.Versioning;
using MediatR;
using NashAssetManagement.Application.Utilities;
using GetUsers = NashAssetManagement.Application.UseCases.Users.ViewList;
using NashAssetManagement.WebAPI.Utilities;

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
    }
}