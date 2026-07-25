using Microsoft.EntityFrameworkCore;
using Payment.Application.Contracts;

namespace Payment.Infrastructure.Persistence.Repositories;

public sealed class PaymentRepository(PaymentDbContext context) : IPaymentRepository
{
    public async Task AddAsync(Domain.Entities.Payment payment, CancellationToken ct) =>
        await context.Payments.AddAsync(payment, ct);

    public Task<Domain.Entities.Payment?> GetByIdAsync(Guid id, CancellationToken ct) =>
        context.Payments.FirstOrDefaultAsync(x => x.Id == id, ct);

    public Task<Domain.Entities.Payment?> GetByOrderIdAsync(Guid orderId, CancellationToken ct) =>
        context.Payments
            .FirstOrDefaultAsync(x => x.OrderId == orderId, ct);

    public Task<Domain.Entities.Payment?> GetByProviderReferenceAsync(
        string providerReference,
        CancellationToken ct) =>
        context.Payments.FirstOrDefaultAsync(
            x => x.ProviderReference == providerReference,
            ct);

    public Task<bool> OrderHasPaymentAsync(Guid orderId, CancellationToken ct) =>
        context.Payments.AnyAsync(x => x.OrderId == orderId, ct);

    public async Task<IReadOnlyList<Domain.Entities.Payment>> GetByUserAsync(
        Guid userId,
        CancellationToken ct) =>
        await context.Payments
            .AsNoTracking()
            .Where(x => x.UserId == userId)
            .OrderByDescending(x => x.CreatedAt)
            .ToListAsync(ct);

    public Task<bool> WebhookEventExistsAsync(string eventId, CancellationToken ct) =>
        context.ProcessedWebhookEvents.AnyAsync(x => x.Id == eventId, ct);

    public async Task AddWebhookEventAsync(
        Domain.Entities.ProcessedWebhookEvent webhookEvent,
        CancellationToken ct) =>
        await context.ProcessedWebhookEvents.AddAsync(webhookEvent, ct);

    public Task<int> SaveChangesAsync(CancellationToken ct) => context.SaveChangesAsync(ct);
}
