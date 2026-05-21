using ErrorOr;
using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using System.Text.RegularExpressions;

namespace NashAssetManagement.WebAPI.Utilities
{
    public static class ProblemDetailsMapper
    {
        public static ProblemDetails FromFluentValidation(
            ValidationException ex)
        {
            var errors = ex.Errors
                .Select(e => new
                {
                    Field = NormalizePropertyName(e.PropertyName),
                    e.ErrorMessage
                })
                .GroupBy(x => x.Field)
                .ToDictionary(
                    g => g.Key,
                    g => g.Select(x => x.ErrorMessage).ToArray());

            var problem = new ProblemDetails
            {
                Title = "Validation error",
                Status = StatusCodes.Status400BadRequest,
                Type = "https://datatracker.ietf.org/doc/html/rfc7231#section-6.5.1",
                Detail = "One or more validation errors occurred."
            };

            problem.Extensions["errors"] = errors;

            return problem;
        }

        private static string NormalizePropertyName(string propertyName)
        {
            // Remove array indexes: Images[0] -> Images
            propertyName = Regex.Replace(propertyName, @"\[\d+\]", "");

            // Convert to camelCase: Price -> price
            return char.ToLowerInvariant(propertyName[0]) + propertyName[1..];
        }

        public static ProblemDetails FromErrorOr(
            List<Error> errors)
        {
            var first = errors[0];
            var status = GetStatusFromErrorType(first.Type);
            return new ProblemDetails
            {
                Title = first.Code,
                Status = status,
                Detail = first.Description,
                Type = GetTypeFromErrorType(first.Type)
            };
        }

        private static int GetStatusFromErrorType(ErrorType errorType)
            => errorType switch
            {
                ErrorType.Validation => StatusCodes.Status400BadRequest,
                ErrorType.Unauthorized => StatusCodes.Status401Unauthorized,
                ErrorType.Forbidden => StatusCodes.Status403Forbidden,
                ErrorType.NotFound => StatusCodes.Status404NotFound,
                ErrorType.Conflict => StatusCodes.Status409Conflict,
                _ => StatusCodes.Status500InternalServerError,
            };

        private static string GetTypeFromErrorType(ErrorType errorType)
            => errorType switch
            {
                ErrorType.Validation => "https://datatracker.ietf.org/doc/html/rfc7231#section-6.5.1",
                ErrorType.Unauthorized => "https://datatracker.ietf.org/doc/html/rfc7235#section-3.1",
                ErrorType.Forbidden => "https://datatracker.ietf.org/doc/html/rfc7231#section-6.5.3",
                ErrorType.NotFound => "https://datatracker.ietf.org/doc/html/rfc7231#section-6.5.4",
                ErrorType.Conflict => "https://datatracker.ietf.org/doc/html/rfc7231#section-6.5.8",
                _ => "https://datatracker.ietf.org/doc/html/rfc7231#section-6.6.1",
            };
    }
}
