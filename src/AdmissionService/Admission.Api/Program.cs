using Admission.Infrastructure.Persistence;
using Admission.Api;
using Admission.Application;
using Admission.Infrastructure;
using BuildingBlocks.Infrastructure.Persistence;
using BuildingBlocks.Observability;

var builder = WebApplication.CreateBuilder(args);
builder.AddDefaultObservability("eventhub.admission");
builder.ConfigureHostUrls();

builder.Services
    .AddInfrastructure(builder.Configuration)
    .AddApplication()
    .AddApiServices(builder.Configuration, builder.Environment);

var app = builder.Build();
await app.Services.MigrateDatabaseAsync<AdmissionDbContext>(
    builder.Configuration.GetValue<bool>("Database:MigrateOnStartup"));
app.UseApiServices();
await app.RunAsync();
