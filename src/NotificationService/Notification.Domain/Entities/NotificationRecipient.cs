using BuildingBlocks.SharedKernel.Abstractions;

namespace Notification.Domain.Entities;

public sealed class NotificationRecipient : Entity<Guid>
{
    private NotificationRecipient() { }

    private NotificationRecipient(Guid userId, string name, string email)
    {
        Id = userId;
        Name = name;
        Email = email;
        IsActive = true;
    }

    public string Name { get; private set; } = string.Empty;
    public string Email { get; private set; } = string.Empty;
    public bool IsActive { get; private set; }

    public static NotificationRecipient Create(Guid userId, string name, string email)
    {
        if (userId == Guid.Empty || string.IsNullOrWhiteSpace(name) || string.IsNullOrWhiteSpace(email))
            throw new DomainException("Recipient user, name and email are required.");
        return new NotificationRecipient(userId, name.Trim(), email.Trim().ToLowerInvariant());
    }

    public void Update(string name, string email)
    {
        if (string.IsNullOrWhiteSpace(name) || string.IsNullOrWhiteSpace(email))
            throw new DomainException("Recipient name and email are required.");
        Name = name.Trim();
        Email = email.Trim().ToLowerInvariant();
        IsActive = true;
    }
}
