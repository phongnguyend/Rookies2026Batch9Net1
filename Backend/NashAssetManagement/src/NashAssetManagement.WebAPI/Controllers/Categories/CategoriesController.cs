using Asp.Versioning;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NashAssetManagement.Application.UseCases.Categories;
using NashAssetManagement.WebAPI.Utilities;

namespace NashAssetManagement.WebAPI.Controllers;

[ApiVersion(1)]
[Authorize]
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