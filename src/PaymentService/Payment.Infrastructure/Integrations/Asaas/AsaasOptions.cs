namespace Payment.Infrastructure.Integrations.Asaas;

public sealed class AsaasOptions
{
    public const string SectionName = "Asaas";

    public string BaseUrl { get; init; } = "https://api-sandbox.asaas.com/v3/";
    public string ApiKey { get; init; } = string.Empty;
    public string WebhookToken { get; init; } = string.Empty;
    public string UserAgent { get; init; } = "EventHub/1.0";
}
