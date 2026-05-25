using Asp.Versioning;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using NashAssetManagement.Application.UseCases.Assets.ViewDetail;
using NashAssetManagement.Application.UseCases.Assets.ViewList;
using NashAssetManagement.Domain.Enums;
using NashAssetManagement.WebAPI.Utilities;

namespace NashAssetManagement.WebAPI.Controllers;

[ApiVersion(1)]
[Route("api/v{version:apiVersion}/assets")]
public class AssetsController : BaseApiController
{
    public AssetsController(ISender sender) : base(sender) { }

    [HttpGet]
    public async Task<IActionResult> GetAll(
        [FromQuery] string? categories,
        [FromQuery] string? states,
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 10,
        CancellationToken cancellationToken = default)
    {
        var categoryList = categories?.Split(",", StringSplitOptions.RemoveEmptyEntries);
        var stateList = states?.Split(",", StringSplitOptions.RemoveEmptyEntries); // ← no Enum.Parse

        var result = await _sender.Send(
            new GetAssetsRequest(categoryList, stateList, pageNumber, pageSize),
            cancellationToken);

        return result.Match(
            assets => Ok(assets),
            errors =>
            {
                var problem = ProblemDetailsMapper.FromErrorOr(errors);
                return new ObjectResult(problem) { StatusCode = problem.Status };
            });
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById(
        Guid id,
        CancellationToken cancellationToken)
    {
        var result = await _sender.Send(
            new GetAssetDetailRequest(id),
            cancellationToken);

        return result.Match(
            asset => Ok(asset),
            errors =>
            {
                var problem = ProblemDetailsMapper.FromErrorOr(errors);
                return new ObjectResult(problem) { StatusCode = problem.Status };
            });
    }
}