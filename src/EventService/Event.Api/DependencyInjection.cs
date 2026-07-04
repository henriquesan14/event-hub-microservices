using AspNetCoreRateLimit;
using BuildingBlocks.Api.Authentication;
using BuildingBlocks.Api.ErrorHandling;
using BuildingBlocks.Api.Extensions;
using BuildingBlocks.SharedKernel.Abstractions;
using Carter;
using HealthChecks.UI.Client;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Scalar.AspNetCore;

namespace Events.Api;

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

    public static IServiceCollection AddApiServices(this IServiceCollection services, IConfiguration configuration, IWebHostEnvironment env)
    {
        services.AddOpenApi();
        services.AddCorsConfig(env);
        services.AddJwtAuthentication(configuration, env);

        services.AddHttpContextAccessor();
        services.AddScoped<IUserContext, UserContext>();

        //services.AddHangfireConfig(configuration);

        services.AddJsonSerializationConfig().AddCarter();

        services.AddExceptionHandler<CustomExceptionHandler>();

        services.AddDefaultOptions(configuration);

        services.AddHealthChecks()
        .AddNpgSql(configuration.GetConnectionString("DbConnection")!);

        services.AddRateLimitingConfig(configuration);

        return services;
    }

    public static WebApplication UseApiServices(this WebApplication app, IConfiguration configuration)
    {
        app.UseExceptionHandler(options => { });

        if (app.Environment.IsDevelopment())
        {
            app.MapOpenApi();
            app.MapScalarApiReference(options =>
            {
                options
                    .WithTitle("Identity API")
                    .WithTheme(ScalarTheme.Purple)
                    .WithDefaultHttpClient(ScalarTarget.CSharp, ScalarClient.HttpClient);
            });
        }

        app.UseCors("AllowSpecificOrigin");

        app.UseAuthentication();
        app.UseAuthorization();

        app.MapCarter();

        app.UseIpRateLimiting();

        //app.UseHangfireDashboardWithAuth(configuration);
        //app.UseRecurringJobs();

        app.UseHealthChecks("/health", new HealthCheckOptions
        {
            ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse
        });

        return app;
    }
}
