using AspNetCoreRateLimit;
using BuildingBlocks.Api.Authentication;
using BuildingBlocks.Api.ErrorHandling;
using BuildingBlocks.Api.Extensions;
using BuildingBlocks.SharedKernel.Abstractions;
using Carter;
using HealthChecks.UI.Client;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Scalar.AspNetCore;

namespace Payment.Api;

public static class DependencyInjection
{
    public static void ConfigureHostUrls(this WebApplicationBuilder builder)
    {
        if (builder.Environment.IsProduction())
        {
            var port = Environment.GetEnvironmentVariable("PORT") ?? "8080";
            builder.WebHost.UseUrls($"http://*:{port}");
        }
    }

    public static IServiceCollection AddApiServices(
        this IServiceCollection services,
        IConfiguration configuration,
        IWebHostEnvironment environment)
    {
        services.AddOpenApi();
        services.AddCorsConfig(environment);
        services.AddJwtAuthentication(configuration, environment);
        services.AddHttpContextAccessor();
        services.AddScoped<IUserContext, UserContext>();
        services.AddJsonSerializationConfig().AddCarter();
        services.AddExceptionHandler<CustomExceptionHandler>();
        services.AddDefaultOptions(configuration);
        services.AddHealthChecks()
            .AddNpgSql(configuration.GetConnectionString("DbConnection")!);
        services.AddRateLimitingConfig(configuration);
        return services;
    }

    public static WebApplication UseApiServices(this WebApplication app)
    {
        app.UseExceptionHandler(options => { });
        if (app.Environment.IsDevelopment())
        {
            app.MapOpenApi();
            app.MapScalarApiReference(options => options
                .WithTitle("Payment API")
                .WithTheme(ScalarTheme.Purple)
                .WithDefaultHttpClient(ScalarTarget.CSharp, ScalarClient.HttpClient));
        }

        app.UseCors("AllowSpecificOrigin");
        app.UseAuthentication();
        app.UseAuthorization();
        app.MapCarter();
        app.UseIpRateLimiting();
        app.UseHealthChecks("/health", new HealthCheckOptions
        {
            ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse
        });
        return app;
    }
}
