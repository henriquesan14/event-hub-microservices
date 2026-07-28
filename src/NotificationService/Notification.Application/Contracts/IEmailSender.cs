namespace Notification.Application.Contracts;

public interface IEmailSender
{
    Task SendAsync(EmailMessage message, CancellationToken ct);
}

public sealed record EmailMessage(
    string RecipientName,
    string RecipientEmail,
    string Subject,
    string TextBody,
    string HtmlBody,
    IReadOnlyList<EmailInlineAttachment>? InlineAttachments = null);

public sealed record EmailInlineAttachment(
    string ContentId,
    string MediaType,
    byte[] Content);
