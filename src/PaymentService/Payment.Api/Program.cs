using BuildingBlocks.Infrastructure.Persistence;
using BuildingBlocks.Observability;
using Payment.Api;
using Payment.Application;
using Payment.Infrastructure;
using Payment.Infrastructure.Persistence;

var builder = WebApplication.CreateBuilder(args);
builder.AddDefaultObservability("eventhub.payments");
builder.ConfigureHostUrls();

builder.Services
    .AddInfrastructure(builder.Configuration)
    .AddApplication()
    .AddApiServices(builder.Configuration, builder.Environment);

var app = builder.Build();
await app.Services.MigrateDatabaseAsync<PaymentDbContext>(
    builder.Configuration.GetValue<bool>("Database:MigrateOnStartup"));
app.UseApiServices();
await app.RunAsync();
