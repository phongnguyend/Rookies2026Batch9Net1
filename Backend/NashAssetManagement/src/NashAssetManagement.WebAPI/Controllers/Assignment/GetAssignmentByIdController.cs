using Asp.Versioning;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NashAssetManagement.Application.UseCases.Assignments.GetById;
using NashAssetManagement.Domain.Constants;
using NashAssetManagement.WebAPI.Utilities;

namespace NashAssetManagement.WebAPI.Controllers.Assignment
{
    [Authorize(Roles = ApplicationRole.Admin)]
    [ApiVersion(1)]
    [Route("api/v{version:apiVersion}/assignments")]
    public sealed class GetAssignmentByIdController(
     ISender sender,
     ILogger<GetAssignmentByIdController> logger)
     : BaseApiController(sender)
    {
        private readonly ILogger<GetAssignmentByIdController> _logger = logger;

        [HttpGet("{id:guid}")]
        [ProducesResponseType(typeof(Response), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetById(Guid id)
        {
            var result = await _sender.Send(new Query(id));

            return result.Match(
                Ok,
                onError: errors => errors.ToProblem()
            );
        }
    }
}
