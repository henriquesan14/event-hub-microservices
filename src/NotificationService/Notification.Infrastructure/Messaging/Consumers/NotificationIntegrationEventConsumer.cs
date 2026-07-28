using BuildingBlocks.Contracts.Orders;
using BuildingBlocks.Contracts.Payments;
using BuildingBlocks.Contracts.Users;
using MassTransit;
using Notification.Application.Contracts;
using Notification.Domain.Enums;

namespace Notification.Infrastructure.Messaging.Consumers;

public sealed class NotificationIntegrationEventConsumer(
    INotificationRepository repository)
    : IConsumer<OrderCreatedIntegrationEvent>,
      IConsumer<OrderCancelledIntegrationEvent>,
      IConsumer<OrderExpiredIntegrationEvent>,
      IConsumer<PaymentApprovedIntegrationEvent>,
      IConsumer<PaymentFailedIntegrationEvent>,
      IConsumer<UserRegisteredIntegrationEvent>,
      IConsumer<UserUpdatedIntegrationEvent>
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

    private async Task AddAsync(
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
            resourceId);
        notification.CreatedBy = userId;
        await repository.AddAsync(notification, ct);
        await repository.AddDeliveryAsync(
            Domain.Entities.NotificationDelivery.Create(notification.Id, userId, DateTime.Now),
            ct);
        await repository.SaveChangesAsync(ct);
    }

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
