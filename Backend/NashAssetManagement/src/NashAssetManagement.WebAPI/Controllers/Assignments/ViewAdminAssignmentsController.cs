using Asp.Versioning;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NashAssetManagement.Application.UseCases.Assignments.GetAll;
using NashAssetManagement.Application.Utilities;
using NashAssetManagement.Domain.Constants;
using NashAssetManagement.WebAPI.Utilities;
using Swashbuckle.AspNetCore.Annotations;

namespace NashAssetManagement.WebAPI.Controllers.Assignments
{
    [Authorize(Roles = ApplicationRole.Admin)]
    [ApiVersion(1)]
    [Route("api/v{version:apiVersion}/admin/assignments")]

    public sealed class ViewAdminAssignmentsController(
        ISender sender) 
        : BaseApiController(sender)
    {
        [HttpGet]
        [ProducesResponseType(typeof(PagedList<Response>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        [SwaggerOperation(
            Summary = "Get admin's assignments",
            Description = "Allow an admin to view all assignments in his/her location. Assignments will be in state 'Waiting for acceptance', 'Accepted', 'Declined' or 'Returned'.",
            Tags = [ControllerTags.Assignments])]
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
