using Microsoft.EntityFrameworkCore;
using Notification.Application.Contracts;
using Notification.Domain.Enums;

namespace Notification.Infrastructure.Persistence.Repositories;

public sealed class NotificationRepository(NotificationDbContext context)
    : INotificationRepository
{
    public async Task AddAsync(Domain.Entities.Notification notification, CancellationToken ct) =>
        await context.Notifications.AddAsync(notification, ct);

    public async Task AddDeliveryAsync(
        Domain.Entities.NotificationDelivery delivery,
        CancellationToken ct) =>
        await context.NotificationDeliveries.AddAsync(delivery, ct);

    public Task<Domain.Entities.NotificationRecipient?> GetRecipientAsync(
        Guid userId,
        CancellationToken ct) =>
        context.NotificationRecipients.FirstOrDefaultAsync(x => x.Id == userId, ct);

    public async Task AddRecipientAsync(
        Domain.Entities.NotificationRecipient recipient,
        CancellationToken ct) =>
        await context.NotificationRecipients.AddAsync(recipient, ct);

    public async Task<IReadOnlyList<Domain.Entities.NotificationDelivery>> GetPendingDeliveriesAsync(
        DateTime now,
        int batchSize,
        CancellationToken ct) =>
        await context.NotificationDeliveries
            .Where(x => x.Status == DeliveryStatus.Pending && x.NextAttemptAt <= now)
            .OrderBy(x => x.NextAttemptAt)
            .Take(batchSize)
            .ToListAsync(ct);

    public Task<Domain.Entities.Notification?> GetByIdAsync(Guid id, CancellationToken ct) =>
        context.Notifications.FirstOrDefaultAsync(x => x.Id == id, ct);

    public async Task<IReadOnlyList<Domain.Entities.Notification>> GetByUserAsync(
        Guid userId,
        CancellationToken ct) =>
        await context.Notifications
            .AsNoTracking()
            .Where(x => x.UserId == userId)
            .OrderByDescending(x => x.CreatedAt)
            .ToListAsync(ct);

    public async Task<IReadOnlyList<Domain.Entities.Notification>> GetUnreadByUserAsync(
        Guid userId,
        CancellationToken ct) =>
        await context.Notifications
            .Where(x => x.UserId == userId && !x.IsRead)
            .ToListAsync(ct);

    public Task<int> CountUnreadAsync(Guid userId, CancellationToken ct) =>
        context.Notifications.CountAsync(x => x.UserId == userId && !x.IsRead, ct);

    public Task<int> SaveChangesAsync(CancellationToken ct) => context.SaveChangesAsync(ct);
}
