using Microsoft.Extensions.DependencyInjection;
using NashAssetManagement.Application.Abstractions.Cookie;

namespace NashAssetManagement.Infrastructure.Cookie
{
    public static class CookieServiceExtensions
    {
        public static IServiceCollection AddCookieService(
            this IServiceCollection services)
        {
            services.AddScoped<ICookieService, CookieService>();

            return services;
        }
    }
}