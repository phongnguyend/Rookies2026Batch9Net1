using Microsoft.Extensions.Options;

namespace NashAssetManagement.WebAPI.Configuration.Cors
{
    public static class CorsServiceCollectionExtensions
    {
        public static IServiceCollection AddCorsServices(
            this IServiceCollection services)
        {
            services.AddOptions<Microsoft.AspNetCore.Cors.Infrastructure.CorsOptions>().Configure<IOptions<CorsOptions>>((libOptions, corsOptions) =>
            {
                libOptions.AddDefaultPolicy(policy =>
               {
                   if (corsOptions.Value.AllowedOrigins.Length > 0)
                   {
                       policy
                           .WithOrigins(
                               corsOptions.Value.AllowedOrigins)
                           .AllowAnyHeader()
                           .AllowAnyMethod()
                           .AllowCredentials();
                   }
               });
            });

            return services;
        }
    }
}