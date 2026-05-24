using Asp.Versioning;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using NashAssetManagement.Application.Utilities;
using NashAssetManagement.WebAPI.Utilities;
using GetAllAssignments = NashAssetManagement.Application.UseCases.Assignments.GetAll;
using GetAssignmentById = NashAssetManagement.Application.UseCases.Assignments.GetById;

namespace NashAssetManagement.WebAPI.Controllers
{
    [ApiVersion(1)]
    [Route("api/v{version:apiVersion}/assignments")]
    public class AssignmentsController
        : BaseApiController
    {
        readonly ILogger<AssignmentsController> _logger;

        public AssignmentsController(
            ISender sender,
            ILogger<AssignmentsController> logger) : base(sender)
        {
            _logger = logger;
        }

        [HttpGet]
        [ProducesResponseType(typeof(PagedList<GetAllAssignments.Response>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetAll([FromQuery] GetAllAssignments.Query query)
        {
            _logger.LogInformation("Getting all assignments with query: {@Query}", query);
            var result = await _sender.Send(query);

            return result.Match(
                onValue: data =>
                {
                    _logger.LogInformation("Successfully retrieved {TotalItems} assignments", data.TotalCount);
                    return Ok(data);
                },
                onError: errors =>
                {
                    _logger.LogWarning("Failed to get assignments: {@Errors}", errors);
                    return errors.ToProblem();
                });
        }

        [HttpGet("{id:guid}")]
        [ProducesResponseType(typeof(GetAssignmentById.Response), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetById(Guid id)
        {
            _logger.LogInformation("Getting assignment by id: {Id}", id);
            var result = await _sender.Send(new GetAssignmentById.Query(id));

            return result.Match(
                onValue: data =>
                {
                    _logger.LogInformation("Successfully retrieved assignment with id: {Id}", id);
                    return Ok(data);
                },
                onError: errors =>
                {
                    _logger.LogWarning("Assignment not found with id: {Id}", id);
                    return errors.ToProblem();
                });
        }
    }
}
