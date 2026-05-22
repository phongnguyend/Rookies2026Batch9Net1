using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using NashAssetManagement.Infrastructure.AppIdentity;
using NashAssetManagement.Infrastructure.DateTimes;

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

            return services;
        }
    }
}
