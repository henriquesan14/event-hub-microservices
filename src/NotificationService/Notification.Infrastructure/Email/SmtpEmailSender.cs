using System.Net;
using System.Net.Mail;
using System.Text;
using Microsoft.Extensions.Options;
using Notification.Application.Contracts;

namespace Notification.Infrastructure.Email;

public sealed class SmtpEmailSender(IOptions<SmtpOptions> options) : IEmailSender
{
    private readonly SmtpOptions _options = options.Value;

    public async Task SendAsync(EmailMessage message, CancellationToken ct)
    {
        ct.ThrowIfCancellationRequested();

        using var mail = new MailMessage
        {
            From = new MailAddress(_options.FromAddress, _options.FromName),
            Subject = message.Subject,
            SubjectEncoding = Encoding.UTF8,
            Body = message.TextBody,
            BodyEncoding = Encoding.UTF8,
            IsBodyHtml = false
        };
        mail.To.Add(new MailAddress(message.RecipientEmail, message.RecipientName));
        var htmlView = AlternateView.CreateAlternateViewFromString(
            message.HtmlBody,
            Encoding.UTF8,
            "text/html");
        foreach (var attachment in message.InlineAttachments ?? [])
        {
            htmlView.LinkedResources.Add(new LinkedResource(
                new MemoryStream(attachment.Content, writable: false),
                attachment.MediaType)
            {
                ContentId = attachment.ContentId,
                TransferEncoding =
                    System.Net.Mime.TransferEncoding.Base64
            });
        }
        mail.AlternateViews.Add(htmlView);

        using var client = new SmtpClient(_options.Host, _options.Port)
        {
            EnableSsl = _options.EnableSsl,
            UseDefaultCredentials = false,
            Credentials = new NetworkCredential(_options.Username, _options.Password)
        };

        await client.SendMailAsync(mail);
    }
}
