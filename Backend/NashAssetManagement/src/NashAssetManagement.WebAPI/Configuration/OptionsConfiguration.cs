using NashAssetManagement.Infrastructure.Jwt;

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
            return services;
        }
    }
}
