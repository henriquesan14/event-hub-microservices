using BuildingBlocks.Contracts.Orders;
using BuildingBlocks.Contracts.Payments;
using BuildingBlocks.Contracts.Users;
using BuildingBlocks.Contracts.Tickets;
using MassTransit;
using Notification.Application.Contracts;
using Notification.Domain.Enums;
using Microsoft.Extensions.Options;
using Notification.Infrastructure.Email;
using System.Net;
using System.Text;
using QRCoder;

namespace Notification.Infrastructure.Messaging.Consumers;

public sealed class NotificationIntegrationEventConsumer(
    INotificationRepository repository,
    IOptions<EmailLinksOptions> emailLinks,
    IEmailSender emailSender,
    IRealtimeNotificationSender realtimeSender)
    : IConsumer<OrderCreatedIntegrationEvent>,
      IConsumer<OrderCancelledIntegrationEvent>,
      IConsumer<OrderExpiredIntegrationEvent>,
      IConsumer<PaymentApprovedIntegrationEvent>,
      IConsumer<PaymentFailedIntegrationEvent>,
      IConsumer<PaymentRefundedIntegrationEvent>,
      IConsumer<UserRegisteredIntegrationEvent>,
      IConsumer<UserUpdatedIntegrationEvent>,
      IConsumer<UserEmailConfirmationRequestedIntegrationEvent>,
      IConsumer<UserPasswordResetRequestedIntegrationEvent>,
      IConsumer<AdmissionTicketsIssuedIntegrationEvent>
{
    public Task Consume(ConsumeContext<OrderCreatedIntegrationEvent> context) =>
        AddAndNotifyRealtimeAsync(
            context.Message.UserId,
            NotificationType.OrderCreated,
            "Pedido criado",
            $"Seu pedido foi criado e aguarda pagamento até {context.Message.ExpiresAt:g}.",
            context.Message.OrderId,
            "/minha-conta",
            context.CancellationToken);

    public Task Consume(ConsumeContext<OrderCancelledIntegrationEvent> context) =>
        AddAndNotifyRealtimeAsync(
            context.Message.UserId,
            NotificationType.OrderCancelled,
            "Pedido cancelado",
            "Seu pedido foi cancelado e os ingressos foram liberados.",
            context.Message.OrderId,
            "/minha-conta",
            context.CancellationToken);

    public Task Consume(ConsumeContext<OrderExpiredIntegrationEvent> context) =>
        AddAndNotifyRealtimeAsync(
            context.Message.UserId,
            NotificationType.OrderExpired,
            "Pedido expirado",
            "O prazo para pagamento expirou e os ingressos foram liberados.",
            context.Message.OrderId,
            "/minha-conta",
            context.CancellationToken);

    public async Task Consume(ConsumeContext<PaymentApprovedIntegrationEvent> context)
    {
        await AddAndNotifyRealtimeAsync(
            context.Message.UserId,
            NotificationType.PaymentApproved,
            "Pagamento aprovado",
            $"Seu pagamento de {context.Message.Amount:N2} {context.Message.Currency} foi aprovado.",
            context.Message.PaymentId,
            "/meus-ingressos",
            context.CancellationToken);
    }

    public Task Consume(ConsumeContext<PaymentRefundedIntegrationEvent> context) =>
        AddAndNotifyRealtimeAsync(
            context.Message.UserId,
            NotificationType.PaymentRefunded,
            "Pagamento estornado",
            $"Seu pagamento de {context.Message.Amount:N2} {context.Message.Currency} foi estornado e os ingressos foram cancelados.",
            context.Message.PaymentId,
            "/minha-conta",
            context.CancellationToken);

    public Task Consume(ConsumeContext<PaymentFailedIntegrationEvent> context) =>
        AddAndNotifyRealtimeAsync(
            context.Message.UserId,
            NotificationType.PaymentFailed,
            "Pagamento não aprovado",
            $"Não foi possível concluir o pagamento: {context.Message.Reason}.",
            context.Message.PaymentId,
            "/minha-conta",
            context.CancellationToken);

    public Task Consume(ConsumeContext<UserRegisteredIntegrationEvent> context) =>
        UpsertRecipientAsync(
            context.Message.UserId,
            context.Message.Name,
            context.Message.Email,
            context.CancellationToken);

    public Task Consume(ConsumeContext<UserUpdatedIntegrationEvent> context) =>
        UpsertRecipientAsync(
            context.Message.UserId,
            context.Message.Name,
            context.Message.Email,
            context.CancellationToken);

    public async Task Consume(
        ConsumeContext<UserEmailConfirmationRequestedIntegrationEvent> context)
    {
        await UpsertRecipientAsync(
            context.Message.UserId,
            context.Message.Name,
            context.Message.Email,
            context.CancellationToken);
        await AddAsync(
            context.Message.UserId,
            NotificationType.EmailConfirmation,
            "Confirme seu e-mail",
            $"Confirme seu endereço de e-mail. Este link expira em {context.Message.ExpiresAt:g}.",
            context.Message.UserId,
            BuildUrl(
                emailLinks.Value.ConfirmEmailPath,
                context.Message.Token),
            context.CancellationToken);
    }

    public async Task Consume(
        ConsumeContext<UserPasswordResetRequestedIntegrationEvent> context)
    {
        await UpsertRecipientAsync(
            context.Message.UserId,
            context.Message.Name,
            context.Message.Email,
            context.CancellationToken);
        await AddAsync(
            context.Message.UserId,
            NotificationType.PasswordReset,
            "Redefinição de senha",
            $"Recebemos uma solicitação para redefinir sua senha. Este link expira em {context.Message.ExpiresAt:g}.",
            context.Message.UserId,
            BuildUrl(
                emailLinks.Value.ResetPasswordPath,
                context.Message.Token),
            context.CancellationToken);
    }

    public async Task Consume(ConsumeContext<AdmissionTicketsIssuedIntegrationEvent> context)
    {
        await AddWithoutEmailDeliveryAsync(
            context.Message.UserId,
            NotificationType.TicketsIssued,
            "Ingressos disponíveis",
            context.Message.Quantity == 1
                ? "Seu ingresso foi emitido e já está disponível."
                : $"Seus {context.Message.Quantity} ingressos foram emitidos e já estão disponíveis.",
            context.Message.OrderId,
            context.CancellationToken);
        await realtimeSender.SendAsync(
            context.Message.UserId,
            NotificationType.TicketsIssued.ToString(),
            "Ingressos disponíveis",
            context.Message.Quantity == 1
                ? "Seu ingresso foi emitido e já está disponível."
                : $"Seus {context.Message.Quantity} ingressos foram emitidos e já estão disponíveis.",
            context.Message.OrderId,
            "/meus-ingressos",
            context.CancellationToken);

        var recipient = await repository.GetRecipientAsync(
            context.Message.UserId,
            context.CancellationToken);
        if (recipient is null || !recipient.IsActive)
            return;

        await emailSender.SendAsync(
            BuildTicketsEmail(recipient.Name, recipient.Email, context.Message),
            context.CancellationToken);
    }

    private Task AddAsync(
        Guid userId,
        NotificationType type,
        string title,
        string message,
        Guid resourceId,
        CancellationToken ct) =>
        AddAsync(userId, type, title, message, resourceId, null, ct);

    private async Task AddAndNotifyRealtimeAsync(
        Guid userId,
        NotificationType type,
        string title,
        string message,
        Guid resourceId,
        string? actionUrl,
        CancellationToken ct)
    {
        await AddAsync(userId, type, title, message, resourceId, ct);
        await realtimeSender.SendAsync(
            userId,
            type.ToString(),
            title,
            message,
            resourceId,
            actionUrl,
            ct);
    }

    private async Task AddWithoutEmailDeliveryAsync(
        Guid userId,
        NotificationType type,
        string title,
        string message,
        Guid resourceId,
        CancellationToken ct)
    {
        var notification = Domain.Entities.Notification.Create(
            userId,
            type,
            title,
            message,
            resourceId,
            actionUrl: null);
        notification.CreatedBy = userId;
        await repository.AddAsync(notification, ct);
        await repository.SaveChangesAsync(ct);
    }

    private async Task AddAsync(
        Guid userId,
        NotificationType type,
        string title,
        string message,
        Guid resourceId,
        string? actionUrl,
        CancellationToken ct)
    {
        var notification = Domain.Entities.Notification.Create(
            userId,
            type,
            title,
            message,
            resourceId,
            actionUrl);
        notification.CreatedBy = userId;
        await repository.AddAsync(notification, ct);
        await repository.AddDeliveryAsync(
            Domain.Entities.NotificationDelivery.Create(notification.Id, userId, DateTime.Now),
            ct);
        await repository.SaveChangesAsync(ct);
    }

    private string BuildUrl(string path, string token) =>
        $"{emailLinks.Value.BaseUrl.TrimEnd('/')}/{path.TrimStart('/')}?token={Uri.EscapeDataString(token)}";

    private async Task UpsertRecipientAsync(
        Guid userId,
        string name,
        string email,
        CancellationToken ct)
    {
        var recipient = await repository.GetRecipientAsync(userId, ct);
        if (recipient is null)
        {
            recipient = Domain.Entities.NotificationRecipient.Create(userId, name, email);
            await repository.AddRecipientAsync(recipient, ct);
        }
        else
        {
            recipient.Update(name, email);
        }

        await repository.SaveChangesAsync(ct);
    }

    private static EmailMessage BuildTicketsEmail(
        string recipientName,
        string recipientEmail,
        AdmissionTicketsIssuedIntegrationEvent message)
    {
        var attachments = new List<EmailInlineAttachment>();
        var ticketBlocks = new StringBuilder();
        var textTickets = new StringBuilder();

        foreach (var ticket in message.Tickets)
        {
            var contentId = $"ticket-{ticket.TicketId:N}";
            var png = PngByteQRCodeHelper.GetQRCode(
                ticket.Code,
                QRCodeGenerator.ECCLevel.M,
                12);
            attachments.Add(new EmailInlineAttachment(contentId, "image/png", png));

            ticketBlocks.Append($"""
                <div style="margin:20px 0;padding:22px;border:1px solid #e4e4e7;border-radius:14px;text-align:center;">
                  <p style="margin:0 0 14px;font-size:18px;font-weight:bold;color:#18181b;">{WebUtility.HtmlEncode(ticket.TicketName)}</p>
                  <img src="cid:{contentId}" width="220" height="220" alt="QR Code do ingresso" style="display:block;width:220px;height:220px;margin:0 auto;" />
                  <p style="margin:14px 0 0;font-size:12px;color:#71717a;">Apresente este QR Code na entrada</p>
                </div>
                """);
            textTickets.AppendLine($"{ticket.TicketName}: {ticket.Code}");
        }

        var encodedName = WebUtility.HtmlEncode(recipientName);
        var encodedEventName = WebUtility.HtmlEncode(message.EventName);
        var eventDate = message.EventStartsAt.ToString("dd/MM/yyyy 'às' HH:mm");
        var html = $"""
            <!doctype html>
            <html lang="pt-BR">
            <body style="margin:0;background:#f4f4f5;font-family:Arial,sans-serif;color:#18181b;">
              <div style="max-width:620px;margin:0 auto;padding:32px 16px;">
                <div style="background:#ffffff;border-radius:18px;padding:32px;">
                  <p style="margin:0;color:#059669;font-weight:bold;">EVENTHUB</p>
                  <h1 style="margin:12px 0 8px;font-size:28px;">Seus ingressos chegaram</h1>
                  <p style="color:#52525b;line-height:1.6;">Olá, {encodedName}! Guarde este e-mail e apresente um QR Code por participante na entrada.</p>
                  <div style="margin:20px 0;padding:18px;background:#f4f4f5;border-radius:12px;">
                    <p style="margin:0 0 6px;font-size:20px;font-weight:bold;color:#18181b;">{encodedEventName}</p>
                    <p style="margin:0;color:#52525b;">{eventDate}</p>
                  </div>
                  {ticketBlocks}
                  <p style="margin:24px 0 0;font-size:12px;color:#71717a;">Esta é uma mensagem automática. Não compartilhe seus QR Codes.</p>
                </div>
              </div>
            </body>
            </html>
            """;

        var text =
            $"Olá, {recipientName}!{Environment.NewLine}{Environment.NewLine}" +
            $"{message.EventName} — {eventDate}{Environment.NewLine}{Environment.NewLine}" +
            $"Seus ingressos EventHub:{Environment.NewLine}{textTickets}" +
            $"{Environment.NewLine}Não compartilhe estes códigos.";

        return new EmailMessage(
            recipientName,
            recipientEmail,
            "Seus ingressos estão disponíveis",
            text,
            html,
            attachments);
    }
}
