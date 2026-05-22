using Microsoft.Extensions.DependencyInjection;
using NashAssetManagement.Application.Abstractions.Jwt;

namespace NashAssetManagement.Infrastructure.Jwt
{
    public static class JwtTokenProviderExtensions
    {
        public static IServiceCollection AddJwtTokenProvider(
            this IServiceCollection services)
        {
            services.AddScoped<IJwtTokenProvider, JwtTokenProvider>();

            return services;
        }
    }
}
