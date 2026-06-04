using Asp.Versioning;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NashAssetManagement.Application.UseCases.Assignments.AdminCreateAssignment;
using NashAssetManagement.Domain.Constants;
using NashAssetManagement.WebAPI.Utilities;
using Swashbuckle.AspNetCore.Annotations;

namespace NashAssetManagement.WebAPI.Controllers.Assignments
{
    [Authorize(Roles = ApplicationRole.Admin)]
    [ApiVersion(1)]
    [Route("api/v{version:apiVersion}/admin/assignments")]
    public class AdminCreateAssignmentController(ISender sender) : BaseApiController(sender)
    {
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]   
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)] 
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]    
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status409Conflict)]     
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        [SwaggerOperation(
            Summary = "Create a new assignment.",
            Description = "Allows an admin to assign an asset to a staff member by creating a new assignment. Asset and Staff member need be the same location with Admin",
            Tags = [ControllerTags.Assignments])]
        public async Task<IActionResult> AdminCreateAssignment(
            [FromBody] Request request,
            CancellationToken cancellationToken)
        {
            var result = await _sender.Send(request, cancellationToken);
            return result.Match(
                _ => Created(),
                errors => errors.ToProblem());
        }
    }
}
