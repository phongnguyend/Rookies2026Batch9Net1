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
        [FromQuery] GetAssetsRequest request,
        CancellationToken cancellationToken = default)
    {
        var result = await _sender.Send(request,cancellationToken);

        return result.Match(
            asset => Ok(asset),
            errors => errors.ToProblem()
        );
    }
}