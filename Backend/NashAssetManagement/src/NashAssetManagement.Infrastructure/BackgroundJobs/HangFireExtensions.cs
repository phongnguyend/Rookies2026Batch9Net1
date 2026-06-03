using Hangfire;
using Hangfire.SqlServer;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using NashAssetManagement.Domain.Constants;

namespace NashAssetManagement.Infrastructure.BackgroundJobs
{
    public static class HangFireExtensions
    {
        const string ConnectionStringName = "DefaultConnection";

        public static IServiceCollection AddHangFireServices(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddHangfire(config =>
            {
                config
                    .SetDataCompatibilityLevel(CompatibilityLevel.Version_180)
                    .UseSimpleAssemblyNameTypeSerializer()
                    .UseRecommendedSerializerSettings()
                    .UseSqlServerStorage(
                        configuration.GetConnectionString(ConnectionStringName),
                        new SqlServerStorageOptions
                        {
                            SchemaName = AppCts.DbSchema.Jobs,
                            QueuePollInterval = TimeSpan.FromSeconds(2), // check continuously all jobs in queue for 3s (near realtime, since database server is weak)
                            SlidingInvisibilityTimeout = TimeSpan.FromMinutes(5), // avoid sql server writing jobs whenever the job is alive
                            UseRecommendedIsolationLevel = true, // avoid deadlock
                            DisableGlobalLocks = true, // avoid database lock from SQL Server, allow parallel workers
                        });
            });

            services.AddHangfireServer(options =>
            {
                options.WorkerCount = 2; // 2 threads are enough for this flow 
            });

            return services;
        }
    }
}
