using Asp.Versioning.ApiExplorer;
using NashAssetManagement.Application;
using NashAssetManagement.Infrastructure;
using NashAssetManagement.Persistence;
using NashAssetManagement.Persistence.SeedData;
using NashAssetManagement.WebAPI;
using Serilog;
using NashAssetManagement.WebAPI.Configuration;
using Microsoft.EntityFrameworkCore;
using NashAssetManagement.Persistence.SeedData;

var builder = WebApplication.CreateBuilder(args);

Log.Logger = new LoggerConfiguration()
    .ReadFrom.Configuration(builder.Configuration)
    .CreateLogger();
builder.Host.UseSerilog();

try
{
    Log.Information("Starting application");
    builder.Services
        .ConfigureApplicationOptions(builder.Configuration)
        .AddApplicationServices(builder.Configuration)
        .AddPersistenceServices(builder.Configuration)
        .AddInfrastructureServices(builder.Configuration)
        .AddApiServices(builder.Configuration);

    var app = builder.Build();

    app.UseSerilogRequestLogging();

    app.UseExceptionHandler();

    if (app.Environment.IsDevelopment())
    {
        app.MapOpenApi();
        app.UseSwagger();
        app.UseSwaggerUI(options =>
        {
            var provider = app.Services.GetRequiredService<IApiVersionDescriptionProvider>();
            foreach (var description in provider.ApiVersionDescriptions)
            {
                options.SwaggerEndpoint(
                    $"/swagger/{description.GroupName}/swagger.json",
                    $"NashAssetManagement API {description.GroupName.ToUpperInvariant()}");
            }
        });
    }
    // // Only uncomment if you need SeedData
    // if (app.Environment.IsDevelopment())
    // {
    //     using var scope = app.Services.CreateScope();
    //     var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    //     Log.Information("Begin seed development data.");
    //     await dbContext.Database.MigrateAsync();
    //     var seeder = scope.ServiceProvider.GetRequiredService<NamDevelopmentSeedData>();
    //     await seeder.SeedDataAsync(scope.ServiceProvider);
    //     Log.Information("Seed development data finished successfully.");    
    // }

    app.UseCors();
    app.UseAuthentication();
    app.UseAuthorization();

    app.MapControllers();

    app.Run();
}
catch (Exception ex)
{
    Log.Fatal(ex, "Application terminated unexpectedly");
}
finally
{
    Log.CloseAndFlush();
}
