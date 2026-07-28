namespace Notification.Application.Contracts;

public interface IRealtimeNotificationSender
{
    Task SendAsync(
        Guid userId,
        string type,
        string title,
        string message,
        Guid resourceId,
        string? actionUrl,
        CancellationToken ct);
}
