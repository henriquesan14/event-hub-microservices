using BuildingBlocks.SharedKernel.Abstractions;
using Notification.Domain.Enums;

namespace Notification.Domain.Entities;

public sealed class Notification : AggregateRoot<Guid>
{
    private Notification() { }

    private Notification(
        Guid id,
        Guid userId,
        NotificationType type,
        string title,
        string message,
        Guid resourceId,
        string? actionUrl)
    {
        Id = id;
        UserId = userId;
        Type = type;
        Title = title;
        Message = message;
        ResourceId = resourceId;
        ActionUrl = actionUrl;
    }

    public Guid UserId { get; private set; }
    public NotificationType Type { get; private set; }
    public string Title { get; private set; } = string.Empty;
    public string Message { get; private set; } = string.Empty;
    public Guid ResourceId { get; private set; }
    public string? ActionUrl { get; private set; }
    public bool IsRead { get; private set; }
    public DateTime? ReadAt { get; private set; }

    public static Notification Create(
        Guid userId,
        NotificationType type,
        string title,
        string message,
        Guid resourceId,
        string? actionUrl = null)
    {
        if (userId == Guid.Empty || resourceId == Guid.Empty)
            throw new DomainException("User and resource are required.");
        if (string.IsNullOrWhiteSpace(title) || string.IsNullOrWhiteSpace(message))
            throw new DomainException("Notification title and message are required.");

        return new Notification(
            Guid.NewGuid(),
            userId,
            type,
            title.Trim(),
            message.Trim(),
            resourceId,
            string.IsNullOrWhiteSpace(actionUrl) ? null : actionUrl.Trim());
    }

    public void MarkAsRead(DateTime now)
    {
        if (IsRead) return;
        IsRead = true;
        ReadAt = now;
    }
}
