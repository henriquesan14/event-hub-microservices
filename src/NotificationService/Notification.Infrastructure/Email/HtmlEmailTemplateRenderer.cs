using System.Globalization;
using System.Net;
using System.Reflection;
using Notification.Application.Contracts;
using Notification.Domain.Enums;

namespace Notification.Infrastructure.Email;

public sealed class HtmlEmailTemplateRenderer : IEmailTemplateRenderer
{
    private const string TemplateResourceName =
        "Notification.Infrastructure.Email.Templates.Notification.html";

    private readonly string _template = LoadTemplate();

    public EmailMessage Render(
        string recipientName,
        string recipientEmail,
        Domain.Entities.Notification notification)
    {
        var presentation = GetPresentation(notification.Type);
        var encodedName = WebUtility.HtmlEncode(recipientName);
        var encodedTitle = WebUtility.HtmlEncode(notification.Title);
        var encodedMessage = WebUtility.HtmlEncode(notification.Message)
            .Replace(Environment.NewLine, "<br>", StringComparison.Ordinal);
        var action = GetAction(notification.Type, notification.ActionUrl);

        var htmlBody = _template
            .Replace("{{preheader}}", encodedTitle, StringComparison.Ordinal)
            .Replace("{{recipientName}}", encodedName, StringComparison.Ordinal)
            .Replace("{{title}}", encodedTitle, StringComparison.Ordinal)
            .Replace("{{message}}", encodedMessage, StringComparison.Ordinal)
            .Replace("{{accentColor}}", presentation.AccentColor, StringComparison.Ordinal)
            .Replace("{{icon}}", presentation.Icon, StringComparison.Ordinal)
            .Replace("{{actionBlock}}", action, StringComparison.Ordinal)
            .Replace(
                "{{year}}",
                DateTime.Now.Year.ToString(CultureInfo.InvariantCulture),
                StringComparison.Ordinal);

        var textBody =
            $"Olá, {recipientName}!{Environment.NewLine}{Environment.NewLine}" +
            $"{notification.Title}{Environment.NewLine}" +
            $"{notification.Message}{Environment.NewLine}{Environment.NewLine}" +
            (string.IsNullOrWhiteSpace(notification.ActionUrl)
                ? string.Empty
                : $"{notification.ActionUrl}{Environment.NewLine}{Environment.NewLine}") +
            "EventHub — esta é uma mensagem automática.";

        return new EmailMessage(
            recipientName,
            recipientEmail,
            notification.Title,
            textBody,
            htmlBody);
    }

    private static TemplatePresentation GetPresentation(NotificationType type) =>
        type switch
        {
            NotificationType.OrderCreated => new("#7C3AED", "🎟️"),
            NotificationType.OrderCancelled => new("#DC2626", "✕"),
            NotificationType.OrderExpired => new("#D97706", "⌛"),
            NotificationType.PaymentApproved => new("#059669", "✓"),
            NotificationType.PaymentFailed => new("#DC2626", "!"),
            NotificationType.EmailConfirmation => new("#2563EB", "✉"),
            NotificationType.PasswordReset => new("#7C3AED", "🔒"),
            NotificationType.TicketsIssued => new("#059669", "🎟️"),
            _ => new("#7C3AED", "•")
        };

    private static string GetAction(NotificationType type, string? actionUrl)
    {
        if (string.IsNullOrWhiteSpace(actionUrl))
            return string.Empty;

        var label = type == NotificationType.EmailConfirmation
            ? "Confirmar e-mail"
            : "Redefinir senha";
        var encodedUrl = WebUtility.HtmlEncode(actionUrl);
        return
            $"""<p style="margin:28px 0 0;"><a href="{encodedUrl}" style="display:inline-block;padding:13px 22px;background:#18181b;color:#fff;text-decoration:none;border-radius:8px;font-weight:bold;">{label}</a></p>""";
    }

    private static string LoadTemplate()
    {
        var assembly = Assembly.GetExecutingAssembly();
        using var stream = assembly.GetManifestResourceStream(TemplateResourceName)
            ?? throw new InvalidOperationException(
                $"Embedded email template '{TemplateResourceName}' was not found.");
        using var reader = new StreamReader(stream);
        return reader.ReadToEnd();
    }

    private sealed record TemplatePresentation(string AccentColor, string Icon);
}
