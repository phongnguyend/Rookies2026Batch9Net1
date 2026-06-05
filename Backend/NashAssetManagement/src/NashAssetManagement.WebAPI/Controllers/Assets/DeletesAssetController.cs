using Asp.Versioning;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NashAssetManagement.Application.UseCases.Assets.Delete;
using NashAssetManagement.Domain.Constants;
using NashAssetManagement.WebAPI.Utilities;
using Swashbuckle.AspNetCore.Annotations;

namespace NashAssetManagement.WebAPI.Controllers.Assets;

[ApiVersion(1)]
[Authorize(Roles = ApplicationRole.Admin)]
[Route("api/v{version:apiVersion}/assets")]
public class DeleteAssetController : BaseApiController
{
    public DeleteAssetController(ISender sender) : base(sender) { }

    [HttpDelete]
    [Authorize]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status409Conflict)]
    [SwaggerOperation(Summary = "Delete a asset.",Tags = [ControllerTags.Assets])]
    public async Task<IActionResult> Delete(
        [FromBody] DeleteAssetRequest request,
        CancellationToken cancellationToken)
    {
        var result = await _sender.Send(request, cancellationToken);

        return result.Match(
            asset => Ok(asset),
            errors => errors.ToProblem()
        );
    }
}