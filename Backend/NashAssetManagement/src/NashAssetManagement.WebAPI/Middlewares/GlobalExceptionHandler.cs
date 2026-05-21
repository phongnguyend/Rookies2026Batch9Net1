using FluentValidation;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using NashAssetManagement.WebAPI.Utilities;

namespace NashAssetManagement.WebAPI.Middlewares
{
    public class GlobalExceptionHandler(
        ILogger<GlobalExceptionHandler> logger)
        : IExceptionHandler
    {
        public async ValueTask<bool> TryHandleAsync(
            HttpContext httpContext,
            Exception exception,
            CancellationToken cancellationToken)
        {
            ProblemDetails problem;
            if (exception is ValidationException validationException)
            {
                problem = ProblemDetailsMapper.FromFluentValidation(validationException);
            }
            else
            {
                logger.LogError(exception, "An unhandled exception occurred: {Message}", exception.Message);
                problem = new ProblemDetails
                {
                    Title = "Unexpected error occurred",
                    Status = StatusCodes.Status500InternalServerError,
                    Type = "https://datatracker.ietf.org/doc/html/rfc7231#section-6.6.1",
                    Detail = "An unexpected error occurred in the server.",
                };
            }
            httpContext.Response.StatusCode = problem.Status!.Value;
            await httpContext.Response.WriteAsJsonAsync(problem, cancellationToken);
            return true;
        }
    }
}
