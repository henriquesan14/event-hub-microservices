using System.Reflection;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using OpenTelemetry.Logs;
using OpenTelemetry.Metrics;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;

namespace BuildingBlocks.Observability;

public static class ObservabilityExtensions
{
    public static WebApplicationBuilder AddDefaultObservability(
        this WebApplicationBuilder builder,
        string serviceName)
    {
        var serviceVersion =
            Assembly.GetEntryAssembly()?.GetName().Version?.ToString() ?? "unknown";
        var otlpEndpoint = builder.Configuration["OTEL_EXPORTER_OTLP_ENDPOINT"];

        var openTelemetry = builder.Services
            .AddOpenTelemetry()
            .ConfigureResource(resource => resource.AddService(
                serviceName,
                serviceVersion: serviceVersion,
                serviceInstanceId: Environment.MachineName))
            .WithTracing(tracing => tracing
                .AddAspNetCoreInstrumentation(options =>
                    options.Filter = context =>
                        !context.Request.Path.StartsWithSegments("/health"))
                .AddHttpClientInstrumentation()
                .AddSource("MassTransit")
                .AddSource("Npgsql"))
            .WithMetrics(metrics => metrics
                .AddAspNetCoreInstrumentation()
                .AddHttpClientInstrumentation()
                .AddRuntimeInstrumentation());

        builder.Logging.AddOpenTelemetry(logging =>
        {
            logging.IncludeFormattedMessage = true;
            logging.IncludeScopes = true;
            logging.ParseStateValues = true;

            if (Uri.TryCreate(otlpEndpoint, UriKind.Absolute, out var endpoint))
                logging.AddOtlpExporter(options => options.Endpoint = endpoint);
        });

        if (Uri.TryCreate(otlpEndpoint, UriKind.Absolute, out var exporterEndpoint))
        {
            openTelemetry
                .WithTracing(tracing => tracing.AddOtlpExporter(
                    options => options.Endpoint = exporterEndpoint))
                .WithMetrics(metrics => metrics.AddOtlpExporter(
                    options => options.Endpoint = exporterEndpoint));
        }

        return builder;
    }
}
