using BuildingBlocks.Infrastructure.Persistence;
using Ticketing.Api;
using Ticketing.Application;
using Ticketing.Infrastructure;
using Ticketing.Infrastructure.Persistence;

var builder = WebApplication.CreateBuilder(args);
builder.ConfigureHostUrls();

builder.Services
    .AddInfrastructure(builder.Configuration)
    .AddApplication()
    .AddApiServices(builder.Configuration, builder.Environment);

var app = builder.Build();
await app.Services.MigrateDatabaseAsync<TicketingDbContext>(
    builder.Configuration.GetValue<bool>("Database:MigrateOnStartup"));
app.UseApiServices();
await app.RunAsync();
