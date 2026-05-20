using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace NashAssetManagement.Application
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddApplicationServices(
            this IServiceCollection services,
            IConfiguration configuration)
        {
            return services;
        }
    }
}
