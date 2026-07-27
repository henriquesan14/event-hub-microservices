using Notification.Application.Dtos;

namespace Notification.Application.Extensions;

public static class NotificationExtensions
{
    public static NotificationDto ToDto(this Domain.Entities.Notification notification) =>
        new(
            notification.Id,
            notification.Type,
            notification.Title,
            notification.Message,
            notification.ResourceId,
            notification.IsRead,
            notification.ReadAt,
            notification.CreatedAt);
}
