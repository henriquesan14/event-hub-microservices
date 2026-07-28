using BuildingBlocks.Infrastructure.Persistence;
using Identity.Api;
using Identity.Infrastructure;
using Identity.Application;
using Identity.Infrastructure.Persistence;

var builder = WebApplication.CreateBuilder(args);

var configuration = builder.Configuration;

builder.Services
    .AddInfrastructure(configuration)
    .AddApplication()
    .AddApiServices(configuration, builder.Environment);


var app = builder.Build();

await app.Services.MigrateDatabaseAsync<IdentityDbContext>(
    configuration.GetValue<bool>("Database:MigrateOnStartup"));
app.UseApiServices(configuration);

await app.RunAsync();
