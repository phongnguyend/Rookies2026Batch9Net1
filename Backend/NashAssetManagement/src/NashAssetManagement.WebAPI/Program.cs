using NashAssetManagement.Application;
using NashAssetManagement.Infrastructure;
using NashAssetManagement.Persistence;
using NashAssetManagement.Persistence.SeedData;
using NashAssetManagement.WebAPI;

var builder = WebApplication.CreateBuilder(args);

builder.Services
    .AddApplicationServices(builder.Configuration)
    .AddPersistenceServices(builder.Configuration)
    .AddInfrastructureServices(builder.Configuration)
    .AddApiServices(builder.Configuration);


var app = builder.Build();
//Seed Data comment this code after run web Api Project once
if (app.Environment.IsDevelopment())
{
    using var scope = app.Services.CreateScope();
    var seeder = scope.ServiceProvider.GetRequiredService<NAMDbContextSeedData>();
    await seeder.SeedDataAsync();
}


app.UseExceptionHandler();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseAuthorization();

app.MapControllers();

app.Run();
