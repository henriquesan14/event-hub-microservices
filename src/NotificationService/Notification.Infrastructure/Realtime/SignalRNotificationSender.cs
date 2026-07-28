using Microsoft.AspNetCore.SignalR;
using Notification.Application.Contracts;

namespace Notification.Infrastructure.Realtime;

public sealed class SignalRNotificationSender(
    IHubContext<NotificationHub> hubContext) : IRealtimeNotificationSender
{
    public Task SendAsync(
        Guid userId,
        string type,
        string title,
        string message,
        Guid resourceId,
        string? actionUrl,
        CancellationToken ct) =>
        hubContext.Clients
            .Group(NotificationHub.UserGroup(userId))
            .SendAsync(
                "notificationReceived",
                new
                {
                    type,
                    title,
                    message,
                    resourceId,
                    actionUrl
                },
                ct);
}
