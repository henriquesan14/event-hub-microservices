using Admission.Application.Contracts;
using BuildingBlocks.Contracts.Payments;
using MassTransit;

namespace Admission.Infrastructure.Messaging.Consumers;

public sealed class PaymentRefundedConsumer(IAdmissionRepository repository)
    : IConsumer<PaymentRefundedIntegrationEvent>
{
    public async Task Consume(ConsumeContext<PaymentRefundedIntegrationEvent> context)
    {
        var tickets = await repository.GetByPaymentIdAsync(
            context.Message.PaymentId,
            context.CancellationToken);
        foreach (var ticket in tickets)
            ticket.Cancel(context.Message.RefundedAt);
        if (tickets.Count > 0)
            await repository.SaveChangesAsync(context.CancellationToken);
    }
}
