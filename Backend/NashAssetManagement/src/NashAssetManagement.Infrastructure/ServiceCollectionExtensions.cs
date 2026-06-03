using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using NashAssetManagement.Infrastructure.AppIdentity;
using NashAssetManagement.Infrastructure.BackgroundJobs;
using NashAssetManagement.Infrastructure.Cookie;
using NashAssetManagement.Infrastructure.DateTimes;
using NashAssetManagement.Infrastructure.Jwt;
using NashAssetManagement.Infrastructure.Report;

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
            services.AddReportServices();
            services.AddHangFireServices(configuration);

            return services;
        }
    }
}
