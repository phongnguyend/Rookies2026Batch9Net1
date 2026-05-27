using Asp.Versioning;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NashAssetManagement.Application.Utilities;
using NashAssetManagement.Application.UseCases.Assignments.GetAll;
using NashAssetManagement.WebAPI.Utilities;
using NashAssetManagement.Domain.Constants;

namespace NashAssetManagement.WebAPI.Controllers.Assignment
{
    [Authorize(Roles = ApplicationRole.Admin)]
    [ApiVersion(1)]
    [Route("api/v{version:apiVersion}/assignments")]

    public sealed class GetAllAssignmentsController(
        ISender sender) 
        : BaseApiController(sender)
    {
        [HttpGet]
        [ProducesResponseType(typeof(PagedList<Response>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
      
        public async Task<IActionResult> GetAll([FromQuery] Query query)
        {
            var result = await _sender.Send(query);

            return result.Match(
                Ok,
                onError: errors => errors.ToProblem()
            );
        }
    }
}
