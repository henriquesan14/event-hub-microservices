using BuildingBlocks.Contracts.Payments;
using BuildingBlocks.SharedKernel.CQRS;
using BuildingBlocks.SharedKernel.Result;
using MassTransit;
using Payment.Application.Contracts;
using Payment.Domain.Entities;
using Payment.Domain.Enums;

namespace Payment.Application.Commands.ProcessAsaasWebhook;

public sealed class ProcessAsaasWebhookCommandHandler(
    IPaymentRepository repository,
    IPublishEndpoint publishEndpoint)
    : ICommandHandler<ProcessAsaasWebhookCommand, Result>
{
    private static readonly HashSet<string> ApprovedEvents =
        ["PAYMENT_CONFIRMED", "PAYMENT_RECEIVED"];

    private static readonly HashSet<string> FailedEvents =
    [
        "PAYMENT_CREDIT_CARD_CAPTURE_REFUSED",
        "PAYMENT_OVERDUE",
        "PAYMENT_DELETED",
        "PAYMENT_BANK_SLIP_CANCELLED"
    ];

    public async Task<Result> Handle(ProcessAsaasWebhookCommand request, CancellationToken ct)
    {
        if (await repository.WebhookEventExistsAsync(request.EventId, ct))
            return Result.Success();

        var now = DateTime.Now;
        var payment = await repository.GetByProviderReferenceAsync(
            request.ProviderPaymentId,
            ct);

        payment?.UpdateBillingType(request.BillingType);

        if (payment is not null && payment.Status == PaymentStatus.Pending)
        {
            if (ApprovedEvents.Contains(request.EventType))
            {
                payment.Approve(request.ProviderPaymentId, now);
                await publishEndpoint.Publish(
                    new PaymentApprovedIntegrationEvent(
                        payment.ReservationId,
                        payment.Id,
                        payment.OrderId,
                        payment.ReservationId,
                        payment.UserId,
                        payment.Amount,
                        payment.Currency,
                        now),
                    context => context.CorrelationId = payment.ReservationId,
                    ct);
            }
            else if (FailedEvents.Contains(request.EventType))
            {
                payment.Fail(request.EventType, now);
                await publishEndpoint.Publish(
                    new PaymentFailedIntegrationEvent(
                        payment.ReservationId,
                        payment.Id,
                        payment.OrderId,
                        payment.ReservationId,
                        payment.UserId,
                        request.EventType,
                        now),
                    context => context.CorrelationId = payment.ReservationId,
                    ct);
            }
        }
        else if (payment is not null && request.EventType == "PAYMENT_REFUNDED" &&
                 payment.Status is PaymentStatus.Approved or PaymentStatus.RefundPending)
        {
            payment.ConfirmRefund(now);
            await publishEndpoint.Publish(
                new PaymentRefundedIntegrationEvent(
                    payment.ReservationId,
                    payment.Id,
                    payment.OrderId,
                    payment.ReservationId,
                    payment.UserId,
                    payment.Amount,
                    payment.Currency,
                    payment.RefundReason,
                    now),
                context => context.CorrelationId = payment.ReservationId,
                ct);
        }

        await repository.AddWebhookEventAsync(
            ProcessedWebhookEvent.Create(request.EventId, request.EventType, now),
            ct);
        await repository.SaveChangesAsync(ct);
        return Result.Success();
    }
}
