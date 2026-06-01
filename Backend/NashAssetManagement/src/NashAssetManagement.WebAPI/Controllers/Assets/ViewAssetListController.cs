using Asp.Versioning;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NashAssetManagement.Application.UseCases.Assets.ViewList;
using NashAssetManagement.Domain.Constants;
using NashAssetManagement.WebAPI.Utilities;
using Swashbuckle.AspNetCore.Annotations;

namespace NashAssetManagement.WebAPI.Controllers.Assets;

[ApiVersion(1)]
[Authorize(Roles = ApplicationRole.Admin)]
[Route("api/v{version:apiVersion}/assets")]
public class ViewAssetListController : BaseApiController
{
    public ViewAssetListController(ISender sender) : base(sender) { }

    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
    [SwaggerOperation(Summary = "View asset list.", Tags = [ControllerTags.Assets])]
    public async Task<IActionResult> GetAll(
        [FromQuery] string? categories,
        [FromQuery] string? states,
        [FromQuery] string? search,
        [FromQuery] string? sortBy,
        [FromQuery] string? sortDirection,
        [FromQuery] bool isCreatedNewAsset,
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 10,
        CancellationToken cancellationToken = default)
    {
        var categoryList = categories?.Split(",", StringSplitOptions.RemoveEmptyEntries);
        var stateList = states?.Split(",", StringSplitOptions.RemoveEmptyEntries); // ← no Enum.Parse

        var result = await _sender.Send(
            new GetAssetsRequest(
                categoryList,
                stateList,
                sortBy,
                sortDirection,
                search,
                isCreatedNewAsset,
                pageNumber,
                pageSize),
                cancellationToken);

        return result.Match(
            asset => Ok(asset),
            errors => errors.ToProblem()
        );
    }
}