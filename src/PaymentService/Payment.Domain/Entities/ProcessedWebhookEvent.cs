using BuildingBlocks.SharedKernel.Abstractions;

namespace Payment.Domain.Entities;

public sealed class ProcessedWebhookEvent : Entity<string>
{
    private ProcessedWebhookEvent() { }

    private ProcessedWebhookEvent(string id, string eventType, DateTime processedAt)
    {
        Id = id;
        EventType = eventType;
        ProcessedAt = processedAt;
    }

    public string EventType { get; private set; } = string.Empty;
    public DateTime ProcessedAt { get; private set; }

    public static ProcessedWebhookEvent Create(string id, string eventType, DateTime processedAt)
    {
        if (string.IsNullOrWhiteSpace(id) || string.IsNullOrWhiteSpace(eventType))
            throw new DomainException("Webhook event id and type are required.");

        return new ProcessedWebhookEvent(id.Trim(), eventType.Trim(), processedAt);
    }
}
