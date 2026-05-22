using Microsoft.OpenApi;
using NashAssetManagement.Application;
using NashAssetManagement.Infrastructure;
using NashAssetManagement.Persistence;
using NashAssetManagement.Persistence.SeedData;
using NashAssetManagement.WebAPI;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

Log.Logger = new LoggerConfiguration()
    .ReadFrom.Configuration(builder.Configuration)
    .CreateLogger();
builder.Host.UseSerilog();


try
{
    Log.Information("Starting application");
    builder.Services
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
    }
    if (app.Environment.IsDevelopment())
    {
        using var scope = app.Services.CreateScope();
        var seeder = scope.ServiceProvider.GetRequiredService<NAMDbContextSeedData>();
        await seeder.SeedDataAsync(scope.ServiceProvider);
    }
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
