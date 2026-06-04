using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using NashAssetManagement.Infrastructure.AppIdentity;
using NashAssetManagement.Infrastructure.AppNamingFormat;
using NashAssetManagement.Infrastructure.Cookie;
using NashAssetManagement.Infrastructure.DateTimes;
using NashAssetManagement.Infrastructure.Jwt;

namespace NashAssetManagement.Infrastructure
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddInfrastructureServices(
            this IServiceCollection services,
            IConfiguration configuration)
        {
            services.AddDateTimeProvider();
            services.AddAppIdentityServices();
            services.AddJwtTokenProvider();
            services.AddCookieService();
            services.AddAppNamingFormatServices();

            return services;
        }
    }
}
