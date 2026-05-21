using ErrorOr;
using Microsoft.AspNetCore.Mvc;

namespace NashAssetManagement.WebAPI.Utilities
{
    public static class ErrorOrExtensions
    {
        public static IActionResult ToProblem(
            this List<Error> errors)
        {
            if (errors.Count == 0)
            {
                return new ObjectResult(new ProblemDetails
                {
                    Title = "An unknown error occurred",
                    Status = StatusCodes.Status500InternalServerError,
                    Type = "https://datatracker.ietf.org/doc/html/rfc7231#section-6.6.1",
                    Detail = "An unknown error occurred in the server."
                })
                {
                    StatusCode = StatusCodes.Status500InternalServerError
                };
            }

            var problem = ProblemDetailsMapper.FromErrorOr(errors);
            return new ObjectResult(problem)
            {
                StatusCode = problem.Status
            };
        }
    }
}
