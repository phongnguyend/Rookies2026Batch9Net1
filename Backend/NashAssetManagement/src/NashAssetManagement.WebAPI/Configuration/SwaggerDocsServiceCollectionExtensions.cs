namespace NashAssetManagement.WebAPI.Configuration
{
    public static class SwaggerDocsServiceCollectionExtensions
    {
        public static IServiceCollection AddSwaggerDocumentation(
            this IServiceCollection services)
        {
            services.AddSwaggerGen();
            services.ConfigureOptions<SwaggerGenOptionsSetup>();

            return services;
        }
    }
}
