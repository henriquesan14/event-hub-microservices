using Ticketing.Api;
using Ticketing.Application;
using Ticketing.Infrastructure;

var builder = WebApplication.CreateBuilder(args);
builder.ConfigureHostUrls();

builder.Services
    .AddInfrastructure(builder.Configuration)
    .AddApplication()
    .AddApiServices(builder.Configuration, builder.Environment);

var app = builder.Build();
app.UseApiServices();
await app.RunAsync();
