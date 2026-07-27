using Notification.Domain.Enums;

namespace Notification.Application.Dtos;

public sealed record NotificationDto(
    Guid Id,
    NotificationType Type,
    string Title,
    string Message,
    Guid ResourceId,
    bool IsRead,
    DateTime? ReadAt,
    DateTime? CreatedAt);
