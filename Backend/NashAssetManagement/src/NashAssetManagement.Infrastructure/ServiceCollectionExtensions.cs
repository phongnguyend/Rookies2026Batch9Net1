using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace NashAssetManagement.Infrastructure
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddInfrastructureServices(
            this IServiceCollection services,
            IConfiguration configuration)
        {
            return services;
        }
    }
}
