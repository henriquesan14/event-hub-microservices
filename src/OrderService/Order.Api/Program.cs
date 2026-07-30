using BuildingBlocks.Infrastructure.Persistence;
using BuildingBlocks.Observability;
using Order.Api;
using Order.Application;
using Order.Infrastructure;
using Order.Infrastructure.Persistence;

var builder = WebApplication.CreateBuilder(args);
builder.AddDefaultObservability("eventhub.orders");
builder.ConfigureHostUrls();

builder.Services
    .AddInfrastructure(builder.Configuration)
    .AddApplication()
    .AddApiServices(builder.Configuration, builder.Environment);

var app = builder.Build();
await app.Services.MigrateDatabaseAsync<OrderDbContext>(
    builder.Configuration.GetValue<bool>("Database:MigrateOnStartup"));
app.UseApiServices();
await app.RunAsync();
