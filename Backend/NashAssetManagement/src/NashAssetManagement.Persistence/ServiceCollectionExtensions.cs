using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace NashAssetManagement.Persistence
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddPersistenceServices(
            this IServiceCollection services,
            IConfiguration configuration)
        {
            return services;
        }
    }
}
