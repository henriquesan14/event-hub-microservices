using Identity.Api;
using Identity.Infrastructure;
using Identity.Application;

var builder = WebApplication.CreateBuilder(args);

var configuration = builder.Configuration;

builder.Services
    .AddInfrastructure(configuration)
    .AddApplication()
    .AddApiServices(configuration, builder.Environment);


var app = builder.Build();

app.UseApiServices(configuration);

await app.RunAsync();
