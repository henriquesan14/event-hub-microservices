using Events.Api;
using Events.Infrastructure;
using EventsApplication;

var builder = WebApplication.CreateBuilder(args);

var configuration = builder.Configuration;

builder.Services
    .AddInfrastructure(configuration)
    .AddApplication()
    .AddApiServices(configuration, builder.Environment);


var app = builder.Build();

app.UseApiServices(configuration);

await app.RunAsync();
