using BuildingBlocks.Infrastructure.Persistence;
using Events.Api;
using Events.Infrastructure;
using Events.Infrastructure.Persistence;
using EventsApplication;

var builder = WebApplication.CreateBuilder(args);

var configuration = builder.Configuration;

builder.Services
    .AddInfrastructure(configuration)
    .AddApplication()
    .AddApiServices(configuration, builder.Environment);


var app = builder.Build();

await app.Services.MigrateDatabaseAsync<EventDbContext>(
    configuration.GetValue<bool>("Database:MigrateOnStartup"));
app.UseApiServices(configuration);

await app.RunAsync();
