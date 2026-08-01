namespace Payment.Application.Contracts;

public interface IPaymentRepository
{
    Task AddAsync(Domain.Entities.Payment payment, CancellationToken ct);
    Task<Domain.Entities.Payment?> GetByIdAsync(Guid id, CancellationToken ct);
    Task<Domain.Entities.Payment?> GetByOrderIdAsync(Guid orderId, CancellationToken ct);
    Task<Domain.Entities.Payment?> GetByProviderReferenceAsync(
        string providerReference,
        CancellationToken ct);
    Task<bool> OrderHasPaymentAsync(Guid orderId, CancellationToken ct);
    Task<IReadOnlyList<Domain.Entities.Payment>> GetByUserAsync(Guid userId, CancellationToken ct);
    Task<IReadOnlyList<Domain.Entities.Payment>> GetAllAsync(CancellationToken ct);
    Task<bool> WebhookEventExistsAsync(string eventId, CancellationToken ct);
    Task AddWebhookEventAsync(Domain.Entities.ProcessedWebhookEvent webhookEvent, CancellationToken ct);
    Task<int> SaveChangesAsync(CancellationToken ct);
}
