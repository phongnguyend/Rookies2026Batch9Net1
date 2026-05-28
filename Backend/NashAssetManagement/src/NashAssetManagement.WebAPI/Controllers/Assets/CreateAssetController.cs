using Asp.Versioning;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NashAssetManagement.Application.UseCases.Assets.Create;
using NashAssetManagement.Domain.Constants;
using NashAssetManagement.WebAPI.Utilities;
using Swashbuckle.AspNetCore.Annotations;

namespace NashAssetManagement.WebAPI.Controllers.Assets;

[ApiVersion(1)]
[Authorize(Roles = ApplicationRole.Admin)]
[Route("api/v{version:apiVersion}/assets")]
public class CreateAssetController : BaseApiController
{
    public CreateAssetController(ISender sender) : base(sender) { }

    [HttpPost]
    [Authorize]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status409Conflict)]
    [SwaggerOperation(Summary = "Create a new asset.",Tags = [ControllerTags.Assets])]
    public async Task<IActionResult> Create(
        [FromBody] CreateAssetRequest request,
        CancellationToken cancellationToken)
    {
        var result = await _sender.Send(request, cancellationToken);

        return result.Match(
            asset => CreatedAtAction(
                actionName: nameof(ViewAssetDetailController.GetById),
                controllerName: "ViewAssetDetail",
                routeValues: new { id = asset.Id },
                value: asset),
            errors =>
                {
                    var problem = ProblemDetailsMapper.FromErrorOr(errors);

                    return new ObjectResult(problem)
                    {
                        StatusCode = problem.Status
                    };
                }
        );
    }
}