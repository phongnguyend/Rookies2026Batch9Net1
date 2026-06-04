using Microsoft.Extensions.DependencyInjection;
using NashAssetManagement.Application.Abstractions.AppNamingFormat;

namespace NashAssetManagement.Infrastructure.AppNamingFormat
{
    public static class AppNamingFormatExtensions
    {
        public static IServiceCollection AddAppNamingFormatServices(this IServiceCollection services)
        {
            services.AddScoped<IAppNamingFormat, AppNamingFormat>();
            return services;
        }
    }
}