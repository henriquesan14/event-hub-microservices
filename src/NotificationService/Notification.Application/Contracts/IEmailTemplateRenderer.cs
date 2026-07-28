using Notification.Domain.Entities;

namespace Notification.Application.Contracts;

public interface IEmailTemplateRenderer
{
    EmailMessage Render(
        string recipientName,
        string recipientEmail,
        Notification.Domain.Entities.Notification notification);
}
