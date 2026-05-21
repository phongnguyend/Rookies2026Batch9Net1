using Microsoft.Extensions.DependencyInjection;
using NashAssetManagement.Application.Abstractions.DateTimes;

namespace NashAssetManagement.Infrastructure.DateTimes
{
    public static class DateTimeProviderExtensions
    {
        public static IServiceCollection AddDateTimeProvider(this IServiceCollection services)
        {
            services.AddSingleton<IDateTimeProvider, DateTimeProvider>();
            return services;
        }
    }
}
