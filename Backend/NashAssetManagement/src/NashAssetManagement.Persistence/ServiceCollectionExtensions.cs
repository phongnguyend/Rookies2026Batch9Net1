using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using NashAssetManagement.Application.Abstractions.DataAccess;
using NashAssetManagement.Domain.Entities.Identity;
using NashAssetManagement.Persistence.DataAccess;
using NashAssetManagement.Persistence.SeedData;
namespace NashAssetManagement.Persistence
{
    public static class ServiceCollectionExtensions
    {
        const string ConnectionStringName = "Database";

        public static IServiceCollection AddPersistenceServices(
            this IServiceCollection services,
            IConfiguration configuration)
        {
            // DbContext
            services.AddDbContext<AppDbContext>(options =>
            {
                // For development
                options.EnableSensitiveDataLogging(true);
                options.UseNpgsql(configuration.GetConnectionString(ConnectionStringName))
                        .UseSnakeCaseNamingConvention();
            });
            services.AddRepositories();
            services.AddUnitOfWork();
            services.AddSeedData();
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
        
        private static IServiceCollection AddSeedData(this IServiceCollection services)
        {
            services.AddScoped<NAMDbContextSeedData>();
            return services;
        }
    }
}
