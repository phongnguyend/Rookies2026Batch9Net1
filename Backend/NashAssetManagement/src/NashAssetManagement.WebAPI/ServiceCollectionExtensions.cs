using NashAssetManagement.WebAPI.Configuration;
using NashAssetManagement.WebAPI.Configuration.Cors;
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
            services.AddAspIdentityServices(); // cookie authentication set by default
            services.AddAuthenticationServices(); // jwt bearer will overwrite the previous, make sure add below the identity server
            services.AddAuthorizationServices();
            services.AddHttpContextAccessor();
            services.ConfigureApiVersioning();
            services.AddSwaggerDocumentation();
            services.AddCorsServices();
            services.ConfigureApplicationOptions(configuration);
            
            return services;
        }
    }
}
