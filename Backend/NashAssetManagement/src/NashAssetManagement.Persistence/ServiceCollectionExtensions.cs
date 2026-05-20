using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using NashAssetManagement.Application.Abstractions.DataAccess;
using NashAssetManagement.Persistence.DataAccess;

namespace NashAssetManagement.Persistence
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddPersistenceServices(
            this IServiceCollection services,
            IConfiguration configuration)
        {
            services.AddRepositories();
            services.AddUnitOfWork();

            return services;
        }

        private static IServiceCollection AddRepositories(
            this IServiceCollection services)
        {
            services.AddScoped(typeof(IRepository<,>), typeof(EFRepository<,>));

            return services;
        }

        private static IServiceCollection AddUnitOfWork(
            this IServiceCollection services)
        {
            services.AddScoped<IUnitOfWork, EFUnitOfWork>();

            return services;
        }
    }
}
