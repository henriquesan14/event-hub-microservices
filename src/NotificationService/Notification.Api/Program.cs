using BuildingBlocks.Infrastructure.Persistence;
using BuildingBlocks.Observability;
using Notification.Api;
using Notification.Application;
using Notification.Infrastructure;
using Notification.Infrastructure.Persistence;

var builder = WebApplication.CreateBuilder(args);
builder.AddDefaultObservability("eventhub.notifications");
builder.ConfigureHostUrls();

builder.Services
    .AddInfrastructure(builder.Configuration)
    .AddApplication()
    .AddApiServices(builder.Configuration, builder.Environment);

var app = builder.Build();
await app.Services.MigrateDatabaseAsync<NotificationDbContext>(
    builder.Configuration.GetValue<bool>("Database:MigrateOnStartup"));
app.UseApiServices();
await app.RunAsync();
