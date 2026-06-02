using Asp.Versioning.ApiExplorer;
using NashAssetManagement.Application;
using NashAssetManagement.Infrastructure;
using NashAssetManagement.Persistence;
using NashAssetManagement.Persistence.SeedData;
using NashAssetManagement.WebAPI;
using Serilog;
using NashAssetManagement.WebAPI.Configuration;
using Microsoft.EntityFrameworkCore;

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

    using var scope = app.Services.CreateScope();
    var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    var dataRemover = scope.ServiceProvider.GetRequiredService<DataResetService>();
    Log.Information("Begin database migrating.");
    await dbContext.Database.MigrateAsync();
    Log.Information("Database migrated successfully.");
    Log.Information("Begin removing data.");
    await dataRemover.ResetDataAsync();
    Log.Information("Data removed successfully.");
    var seeder = scope.ServiceProvider.GetRequiredService<NamDevelopmentSeedData>();
    Log.Information("Begin seeding development data.");
    await seeder.SeedDataAsync(scope.ServiceProvider);
    Log.Information("Seed development data successfully.");

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
