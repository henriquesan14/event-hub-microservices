using BuildingBlocks.Contracts.Orders;
using BuildingBlocks.Contracts.Payments;
using BuildingBlocks.Contracts.Users;
using BuildingBlocks.Contracts.Tickets;
using MassTransit;
using Notification.Application.Contracts;
using Notification.Domain.Enums;
using Microsoft.Extensions.Options;
using Notification.Infrastructure.Email;

namespace Notification.Infrastructure.Messaging.Consumers;

public sealed class NotificationIntegrationEventConsumer(
    INotificationRepository repository,
    IOptions<EmailLinksOptions> emailLinks)
    : IConsumer<OrderCreatedIntegrationEvent>,
      IConsumer<OrderCancelledIntegrationEvent>,
      IConsumer<OrderExpiredIntegrationEvent>,
      IConsumer<PaymentApprovedIntegrationEvent>,
      IConsumer<PaymentFailedIntegrationEvent>,
      IConsumer<UserRegisteredIntegrationEvent>,
      IConsumer<UserUpdatedIntegrationEvent>,
      IConsumer<UserEmailConfirmationRequestedIntegrationEvent>,
      IConsumer<UserPasswordResetRequestedIntegrationEvent>,
      IConsumer<AdmissionTicketsIssuedIntegrationEvent>
{
    public Task Consume(ConsumeContext<OrderCreatedIntegrationEvent> context) =>
        AddAsync(
            context.Message.UserId,
            NotificationType.OrderCreated,
            "Pedido criado",
            $"Seu pedido foi criado e aguarda pagamento até {context.Message.ExpiresAt:g}.",
            context.Message.OrderId,
            context.CancellationToken);

    public Task Consume(ConsumeContext<OrderCancelledIntegrationEvent> context) =>
        AddAsync(
            context.Message.UserId,
            NotificationType.OrderCancelled,
            "Pedido cancelado",
            "Seu pedido foi cancelado e os ingressos foram liberados.",
            context.Message.OrderId,
            context.CancellationToken);

    public Task Consume(ConsumeContext<OrderExpiredIntegrationEvent> context) =>
        AddAsync(
            context.Message.UserId,
            NotificationType.OrderExpired,
            "Pedido expirado",
            "O prazo para pagamento expirou e os ingressos foram liberados.",
            context.Message.OrderId,
            context.CancellationToken);

    public Task Consume(ConsumeContext<PaymentApprovedIntegrationEvent> context) =>
        AddAsync(
            context.Message.UserId,
            NotificationType.PaymentApproved,
            "Pagamento aprovado",
            $"Seu pagamento de {context.Message.Amount:N2} {context.Message.Currency} foi aprovado.",
            context.Message.PaymentId,
            context.CancellationToken);

    public Task Consume(ConsumeContext<PaymentFailedIntegrationEvent> context) =>
        AddAsync(
            context.Message.UserId,
            NotificationType.PaymentFailed,
            "Pagamento não aprovado",
            $"Não foi possível concluir o pagamento: {context.Message.Reason}.",
            context.Message.PaymentId,
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

    public Task Consume(ConsumeContext<AdmissionTicketsIssuedIntegrationEvent> context) =>
        AddAsync(
            context.Message.UserId,
            NotificationType.TicketsIssued,
            "Ingressos disponíveis",
            context.Message.Quantity == 1
                ? "Seu ingresso foi emitido e já está disponível."
                : $"Seus {context.Message.Quantity} ingressos foram emitidos e já estão disponíveis.",
            context.Message.OrderId,
            context.CancellationToken);

    private Task AddAsync(
        Guid userId,
        NotificationType type,
        string title,
        string message,
        Guid resourceId,
        CancellationToken ct) =>
        AddAsync(userId, type, title, message, resourceId, null, ct);

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
}
