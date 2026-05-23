using NashAssetManagement.WebAPI.Configuration;
using NashAssetManagement.WebAPI.Middlewares;
using NashAssetManagement.WebAPI.Utilities.Converters;
using System.Text.Json.Serialization;

namespace NashAssetManagement.WebAPI
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddApiServices(
            this IServiceCollection services, IConfiguration configuration)
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
            services.AddAuthorizationServices();
            services.AddHttpContextAccessor();
            services.AddAspIdentityServices();
            services.ConfigureApiVersioning();
            services.AddSwaggerDocumentation();

            return services;
        }
    }
}
