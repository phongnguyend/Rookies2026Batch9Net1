// using Asp.Versioning;
// using MediatR;
// using Microsoft.AspNetCore.Authorization;
// using Microsoft.AspNetCore.Mvc;
// using NashAssetManagement.Application.UseCases.Assets.ViewDetail;
// using NashAssetManagement.Domain.Constants;
// using NashAssetManagement.WebAPI.Utilities;

// namespace NashAssetManagement.WebAPI.Controllers.Assets;

// [ApiVersion(1)]
// [Authorize(Roles = ApplicationRole.Admin)]
// [Route("api/v{version:apiVersion}/assets")]
// public class ViewAssetDetailController : BaseApiController
// {
//     public ViewAssetDetailController(ISender sender) : base(sender) { }

//     [HttpGet("{id:guid}")]
//     [ProducesResponseType(StatusCodes.Status200OK)]
//     [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
//     [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
//     [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status403Forbidden)]
//     [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
//     [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
//     public async Task<IActionResult> GetById(
//         Guid id,
//     CancellationToken cancellationToken)
//     {
//         var result = await _sender.Send(
//             new GetAssetDetailRequest(id.ToString()),
//             cancellationToken);

//         return result.Match(
//             asset => Ok(asset),
//             errors =>
//             {
//                 var problem = ProblemDetailsMapper.FromErrorOr(errors);

//                 return new ObjectResult(problem)
//                 {
//                     StatusCode = problem.Status
//                 };
//             });
//     }
// }