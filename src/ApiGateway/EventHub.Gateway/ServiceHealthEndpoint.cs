using System.Net;

namespace EventHub.Gateway;

public static class ServiceHealthEndpoint
{
    public static async Task<IResult> HandleAsync(
        IConfiguration configuration,
        IHttpClientFactory httpClientFactory,
        CancellationToken ct)
    {
        var services = configuration
            .GetSection("ServiceHealth")
            .Get<Dictionary<string, string>>() ?? [];
        var client = httpClientFactory.CreateClient();

        var checks = await Task.WhenAll(services.Select(async service =>
        {
            try
            {
                using var response = await client.GetAsync(service.Value, ct);
                return new ServiceHealth(
                    service.Key,
                    response.IsSuccessStatusCode ? "Healthy" : "Unhealthy",
                    (int)response.StatusCode);
            }
            catch
            {
                return new ServiceHealth(
                    service.Key,
                    "Unhealthy",
                    (int)HttpStatusCode.ServiceUnavailable);
            }
        }));

        var healthy = checks.All(x => x.Status == "Healthy");
        return Results.Json(
            new
            {
                status = healthy ? "Healthy" : "Unhealthy",
                services = checks
            },
            statusCode: healthy
                ? StatusCodes.Status200OK
                : StatusCodes.Status503ServiceUnavailable);
    }

    private sealed record ServiceHealth(string Name, string Status, int StatusCode);
}
