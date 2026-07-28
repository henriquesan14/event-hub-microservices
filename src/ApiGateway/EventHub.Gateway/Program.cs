using System.Threading.RateLimiting;
using EventHub.Gateway;
using Microsoft.AspNetCore.HttpOverrides;

var builder = WebApplication.CreateBuilder(args);

if (builder.Environment.IsProduction())
{
    var port = Environment.GetEnvironmentVariable("PORT") ?? "8080";
    builder.WebHost.UseUrls($"http://*:{port}");
}

builder.Services
    .AddReverseProxy()
    .LoadFromConfig(builder.Configuration.GetSection("ReverseProxy"));

builder.Services.AddCors(options =>
{
    options.AddPolicy("GatewayCors", policy =>
    {
        var origins = builder.Configuration
            .GetSection("Cors:AllowedOrigins")
            .Get<string[]>() ?? [];

        if (origins.Length > 0)
            policy.WithOrigins(origins);
        else
            policy.SetIsOriginAllowed(_ => builder.Environment.IsDevelopment());

        policy.AllowAnyHeader()
            .AllowAnyMethod()
            .AllowCredentials();
    });
});

builder.Services.AddRateLimiter(options =>
{
    options.RejectionStatusCode = StatusCodes.Status429TooManyRequests;
    options.GlobalLimiter = PartitionedRateLimiter.Create<HttpContext, string>(
        context => RateLimitPartition.GetFixedWindowLimiter(
            context.Connection.RemoteIpAddress?.ToString() ?? "unknown",
            _ => new FixedWindowRateLimiterOptions
            {
                PermitLimit = builder.Configuration.GetValue("RateLimiting:PermitLimit", 200),
                Window = TimeSpan.FromSeconds(
                    builder.Configuration.GetValue("RateLimiting:WindowSeconds", 60)),
                QueueLimit = 0,
                AutoReplenishment = true
            }));
});

builder.Services.AddHttpClient();
builder.Services.AddHealthChecks();

var app = builder.Build();

app.UseForwardedHeaders(new ForwardedHeadersOptions
{
    ForwardedHeaders =
        ForwardedHeaders.XForwardedFor |
        ForwardedHeaders.XForwardedProto
});
app.UseMiddleware<CorrelationIdMiddleware>();
app.UseCors("GatewayCors");
app.UseRateLimiter();

app.MapGet("/", () => Results.Ok(new
{
    name = "EventHub API Gateway",
    status = "running",
    routes = new
    {
        identity = new[] { "/api/auth", "/api/users" },
        events = new[] { "/api/events" },
        ticketing = new[] { "/api/tickets", "/api/reservations", "/api/events/{eventId}/tickets" },
        orders = new[] { "/api/orders" },
        payments = new[] { "/api/payments", "/api/webhooks/asaas" },
        admission = new[] { "/api/admission" },
        notifications = new[] { "/api/notifications" }
    }
}));

app.MapHealthChecks("/health");
app.MapGet("/health/services", ServiceHealthEndpoint.HandleAsync);
app.MapReverseProxy();

await app.RunAsync();
