using Asp.Versioning.ApiExplorer;
using Microsoft.Extensions.Options;
using Microsoft.OpenApi;
using Swashbuckle.AspNetCore.Annotations;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace NashAssetManagement.WebAPI.Configuration
{
    public class SwaggerGenOptionsSetup(
        IApiVersionDescriptionProvider provider)
        : IConfigureOptions<SwaggerGenOptions>
    {
        public void Configure(SwaggerGenOptions options)
        {
            options.CustomSchemaIds(type => type.FullName);

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
                options.TagActionsBy(api =>
                {
                    // 1) If an explicit SwaggerOperationAttribute provides tags, use them.
                    var explicitTags = api.ActionDescriptor?.EndpointMetadata?
                        .OfType<SwaggerOperationAttribute>()
                        .SelectMany(a => a.Tags ?? Array.Empty<string>())
                        .Where(t => !string.IsNullOrWhiteSpace(t))
                        .ToArray();

                    if (explicitTags != null && explicitTags.Length > 0)
                        return explicitTags;

                    // 2) Fallback to controller route value (typical for controller-based endpoints)
                    if (api.ActionDescriptor?.RouteValues != null &&
                        api.ActionDescriptor.RouteValues.TryGetValue("controller", out var controllerName) &&
                        !string.IsNullOrWhiteSpace(controllerName))
                    {
                        return [controllerName];
                    }

                    // 3) Final fallback: group by HTTP method + path so tag list isn't empty
                    return [$"{api.HttpMethod ?? "UNKN"} {api.RelativePath}"];
                });
            }
        }
    }
}
