namespace Notification.Application.Contracts;

public interface INotificationRepository
{
    Task AddAsync(Domain.Entities.Notification notification, CancellationToken ct);
    Task AddDeliveryAsync(Domain.Entities.NotificationDelivery delivery, CancellationToken ct);
    Task<Domain.Entities.NotificationRecipient?> GetRecipientAsync(Guid userId, CancellationToken ct);
    Task AddRecipientAsync(Domain.Entities.NotificationRecipient recipient, CancellationToken ct);
    Task<IReadOnlyList<Domain.Entities.NotificationDelivery>> GetPendingDeliveriesAsync(
        DateTime now,
        int batchSize,
        CancellationToken ct);
    Task<Domain.Entities.Notification?> GetByIdAsync(Guid id, CancellationToken ct);
    Task<IReadOnlyList<Domain.Entities.Notification>> GetByUserAsync(
        Guid userId,
        CancellationToken ct);
    Task<IReadOnlyList<Domain.Entities.Notification>> GetUnreadByUserAsync(
        Guid userId,
        CancellationToken ct);
    Task<int> CountUnreadAsync(Guid userId, CancellationToken ct);
    Task<int> SaveChangesAsync(CancellationToken ct);
}
