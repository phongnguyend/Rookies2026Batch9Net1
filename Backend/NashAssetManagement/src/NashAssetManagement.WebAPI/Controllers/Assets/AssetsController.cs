using Asp.Versioning;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NashAssetManagement.Application.UseCases.Assets.ViewDetail;
using NashAssetManagement.Application.UseCases.Assets.ViewList;
using NashAssetManagement.Domain.Constants;
using NashAssetManagement.WebAPI.Utilities;
using Swashbuckle.AspNetCore.Annotations;

namespace NashAssetManagement.WebAPI.Controllers;

[ApiVersion(1)]
[Authorize(Roles = ApplicationRole.Admin)]
[Route("api/v{version:apiVersion}/assets")]
public class AssetsController : BaseApiController
{
    public AssetsController(ISender sender) : base(sender) { }

    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
    [SwaggerOperation(
        Summary = "Get all assets.",
        Tags = [ControllerTags.Assets])]
    public async Task<IActionResult> GetAll(
        [FromQuery] string? categories,
        [FromQuery] string? states,
        [FromQuery] string? search,
        [FromQuery] string? sortBy,
        [FromQuery] string? sortDirection,
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 10,
        CancellationToken cancellationToken = default)
    {
        var categoryList = categories?.Split(",", StringSplitOptions.RemoveEmptyEntries);
        var stateList = states?.Split(",", StringSplitOptions.RemoveEmptyEntries); // ← no Enum.Parse

        var result = await _sender.Send(
            new GetAssetsRequest(categoryList, stateList, sortBy, sortDirection, search, pageNumber, pageSize),
            cancellationToken);

        return result.Match(
            asset => Ok(asset),
            errors => errors.ToProblem()
        );
    }

    [HttpGet("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
    [SwaggerOperation(Tags = [ControllerTags.Assets])]
    public async Task<IActionResult> GetById(
        string? id,
        CancellationToken cancellationToken)
    {
        var result = await _sender.Send(
            new GetAssetDetailRequest(id),
            cancellationToken);

        return result.Match(
            asset => Ok(asset),
            errors => errors.ToProblem()
        );
    }
}