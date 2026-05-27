using Asp.Versioning;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NashAssetManagement.Application.UseCases.Categories.ViewList;
using NashAssetManagement.Domain.Constants;
using NashAssetManagement.WebAPI.Utilities;

namespace NashAssetManagement.WebAPI.Controllers;

[ApiVersion(1)]
[Authorize(Roles = ApplicationRole.Admin)]
[ProducesResponseType(StatusCodes.Status200OK)]
[ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
[ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
[ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status403Forbidden)]
[ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
[ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
[Route("api/v{version:apiVersion}/categories")]
public class CategoriesController : BaseApiController
{
    public CategoriesController(ISender sender) : base(sender) { }

    [HttpGet]
    public async Task<IActionResult> GetAll(CancellationToken cancellationToken)
    {
        var result = await _sender.Send(new GetCategoriesRequest(), cancellationToken);

        return result.Match(
            categories => Ok(categories),
            errors =>
            {
                var problem = ProblemDetailsMapper.FromErrorOr(errors);
                return new ObjectResult(problem) { StatusCode = problem.Status };
            });
    }
}