using BuildingBlocks.SharedKernel.Abstractions;
using Notification.Domain.Enums;

namespace Notification.Domain.Entities;

public sealed class NotificationDelivery : Entity<Guid>
{
    private NotificationDelivery() { }

    private NotificationDelivery(Guid notificationId, Guid userId, DateTime now)
    {
        Id = Guid.NewGuid();
        NotificationId = notificationId;
        UserId = userId;
        Status = DeliveryStatus.Pending;
        NextAttemptAt = now;
    }

    public Guid NotificationId { get; private set; }
    public Guid UserId { get; private set; }
    public DeliveryStatus Status { get; private set; }
    public int AttemptCount { get; private set; }
    public DateTime NextAttemptAt { get; private set; }
    public DateTime? LastAttemptAt { get; private set; }
    public DateTime? SentAt { get; private set; }
    public string? LastError { get; private set; }

    public static NotificationDelivery Create(Guid notificationId, Guid userId, DateTime now)
    {
        if (notificationId == Guid.Empty || userId == Guid.Empty)
            throw new DomainException("Notification and user are required.");
        return new NotificationDelivery(notificationId, userId, now);
    }

    public void MarkSent(DateTime now)
    {
        Status = DeliveryStatus.Sent;
        AttemptCount++;
        LastAttemptAt = now;
        SentAt = now;
        LastError = null;
    }

    public void MarkFailed(string error, DateTime now, int maxAttempts)
    {
        AttemptCount++;
        LastAttemptAt = now;
        LastError = string.IsNullOrWhiteSpace(error) ? "Unknown email error" : error[..Math.Min(error.Length, 2000)];
        Status = AttemptCount >= maxAttempts ? DeliveryStatus.Failed : DeliveryStatus.Pending;
        var delayMinutes = Math.Min(Math.Pow(2, AttemptCount), 60);
        NextAttemptAt = now.AddMinutes(delayMinutes);
    }
}
