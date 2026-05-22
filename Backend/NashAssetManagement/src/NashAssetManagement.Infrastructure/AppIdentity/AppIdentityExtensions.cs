using Microsoft.Extensions.DependencyInjection;
using NashAssetManagement.Application.Abstractions.AppIdentity;

namespace NashAssetManagement.Infrastructure.AppIdentity
{
    public static class AppIdentityExtensions
    {
        public static IServiceCollection AddAppIdentityServices(this IServiceCollection services)
        {
            services.AddScoped<ICurrentUser, CurrentUser>();
            return services;
        }
    }
}
