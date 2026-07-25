using System.Security.Cryptography;
using System.Text;
using Carter;
using MediatR;
using Microsoft.Extensions.Options;
using Payment.Application.Commands.ProcessAsaasWebhook;
using Payment.Infrastructure.Integrations.Asaas;

namespace Payment.Api.Endpoints;

public sealed class AsaasWebhookModule : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapPost("/api/webhooks/asaas", ProcessWebhook)
            .AllowAnonymous();
    }

    private static async Task<IResult> ProcessWebhook(
        AsaasWebhookRequest request,
        HttpContext httpContext,
        IOptions<AsaasOptions> options,
        ISender sender,
        CancellationToken ct)
    {
        var receivedToken = httpContext.Request.Headers["asaas-access-token"].ToString();
        if (!TokenMatches(receivedToken, options.Value.WebhookToken))
            return Results.Unauthorized();

        await sender.Send(
            new ProcessAsaasWebhookCommand(
                request.Id,
                request.Event,
                request.Payment.Id),
            ct);
        return Results.Ok();
    }

    private static bool TokenMatches(string received, string expected)
    {
        if (string.IsNullOrWhiteSpace(received) || string.IsNullOrWhiteSpace(expected))
            return false;

        var receivedBytes = Encoding.UTF8.GetBytes(received);
        var expectedBytes = Encoding.UTF8.GetBytes(expected);
        return receivedBytes.Length == expectedBytes.Length &&
               CryptographicOperations.FixedTimeEquals(receivedBytes, expectedBytes);
    }

    private sealed record AsaasWebhookRequest(
        string Id,
        string Event,
        AsaasWebhookPayment Payment);

    private sealed record AsaasWebhookPayment(string Id);
}
