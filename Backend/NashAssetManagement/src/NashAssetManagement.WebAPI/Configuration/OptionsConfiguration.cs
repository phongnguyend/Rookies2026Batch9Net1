using NashAssetManagement.Infrastructure.Jwt;
using NashAssetManagement.WebAPI.Configuration.Cors;

namespace NashAssetManagement.WebAPI.Configuration
{
    public static class OptionsConfiguration
    {
        public static IServiceCollection ConfigureApplicationOptions(
            this IServiceCollection services,
            IConfiguration configuration)
        {
            services.AddOptions<JwtOptions>()
                .BindConfiguration(JwtOptions.SectionName)
                .ValidateDataAnnotations()
                .ValidateOnStart();

            services.AddOptions<CorsOptions>()
                .BindConfiguration(CorsOptions.SectionName)
                .ValidateDataAnnotations()
                .ValidateOnStart();

            return services;
        }
    }
}
