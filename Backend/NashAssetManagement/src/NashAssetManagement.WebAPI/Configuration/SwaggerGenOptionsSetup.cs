using Asp.Versioning.ApiExplorer;
using Microsoft.Extensions.Options;
using Microsoft.OpenApi;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace NashAssetManagement.WebAPI.Configuration
{
    public class SwaggerGenOptionsSetup(
        IApiVersionDescriptionProvider provider)
        : IConfigureOptions<SwaggerGenOptions>
    {
        public void Configure(SwaggerGenOptions options)
        {
            foreach (var description in provider.ApiVersionDescriptions)
            {
                options.EnableAnnotations();
                options.SwaggerDoc(
                    description.GroupName,
                    new OpenApiInfo
                    {
                        Title = "NashAssetManagement API",
                        Version = description.ApiVersion.ToString(),
                        Description = "API documentation for NashAssetManagement system.",
                        Contact = new OpenApiContact
                        {
                            Name = "Developer Team",
                            Url = new Uri("https://github.com/phongnguyend/Rookies2026Batch9Net1"),
                        }
                    });
                options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                {
                    Name = "Authorization",
                    Type = SecuritySchemeType.Http,
                    Scheme = "Bearer",
                    BearerFormat = "JWT",
                    In = ParameterLocation.Header,
                    Description = "JWT Authorization header using Bearer scheme.",
                });
                options.AddSecurityRequirement(document => new OpenApiSecurityRequirement
                {
                    {
                        new OpenApiSecuritySchemeReference("Bearer", document),
                        new List<string>()
                    }
                });
            }
        }
    }
}
