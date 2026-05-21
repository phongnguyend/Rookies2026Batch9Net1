using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Mvc;
using NashAssetManagement.WebAPI.Middlewares;
using NashAssetManagement.WebAPI.Utilities.Converters;
using System.Text.Json.Serialization;

namespace NashAssetManagement.WebAPI
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddApiServices(
            this IServiceCollection services,
            IConfiguration configuration)
        {
            services.AddControllers()
                .AddJsonOptions(options =>
                {
                    options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
                    options.JsonSerializerOptions.Converters.Add(new UtcDateTimeJsonConverter());
                    options.JsonSerializerOptions.Converters.Add(new NullableUtcDateTimeJsonConverter());
                });
            services.AddOpenApi();
            services.AddProblemDetails();
            services.AddExceptionHandler<GlobalExceptionHandler>();
            services.AddAuthenticationServices();

            return services;
        }

        private static IServiceCollection AddAuthenticationServices(
            this IServiceCollection services)
        {
            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
                .AddJwtBearer(options =>
                {
                    options.Events = new JwtBearerEvents
                    {
                        OnChallenge = async context =>
                        {
                            context.HandleResponse();
                            context.Response.Clear();
                            context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                            context.Response.Headers.Append("WWW-Authenticate", "Bearer");
                            context.Response.ContentType = "application/problem+json";
                            var problem = new ProblemDetails
                            {
                                Title = "Unauthorized",
                                Status = StatusCodes.Status401Unauthorized,
                                Type = "https://datatracker.ietf.org/doc/html/rfc7235#section-3.1",
                                Detail = "Authentication is required to access this resource."
                            };
                            await context.Response.WriteAsJsonAsync(problem);
                        },
                        OnForbidden = async context =>
                        {
                            context.Response.Clear();
                            context.Response.StatusCode = StatusCodes.Status403Forbidden;
                            context.Response.ContentType = "application/problem+json";
                            var problem = new ProblemDetails
                            {
                                Title = "Forbidden",
                                Status = StatusCodes.Status403Forbidden,
                                Type = "https://datatracker.ietf.org/doc/html/rfc7231#section-6.5.3",
                                Detail = "You do not have permission to perform this action."
                            };
                            await context.Response.WriteAsJsonAsync(problem);
                        }
                    };
                });
            return services;
        }
    }
}
