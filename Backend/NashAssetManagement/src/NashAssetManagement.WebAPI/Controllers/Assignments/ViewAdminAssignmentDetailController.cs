using Asp.Versioning;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NashAssetManagement.Application.UseCases.Assignments.GetById;
using NashAssetManagement.Domain.Constants;
using NashAssetManagement.WebAPI.Utilities;

namespace NashAssetManagement.WebAPI.Controllers.Assignments
{
    [Authorize(Roles = ApplicationRole.Admin)]
    [ApiVersion(1)]
    [Route("api/v{version:apiVersion}/admin/assignments")]
    public sealed class ViewAdminAssignmentDetailController(
     ISender sender,
     ILogger<ViewAdminAssignmentDetailController> logger)
     : BaseApiController(sender)
    {
        private readonly ILogger<ViewAdminAssignmentDetailController> _logger = logger;

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
