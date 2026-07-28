using System.Security.Cryptography;
using Admission.Application.Contracts;
using Admission.Domain.Entities;
using BuildingBlocks.Contracts.Tickets;
using MassTransit;

namespace Admission.Infrastructure.Messaging.Consumers;

public sealed class ReservationConfirmedConsumer(IAdmissionRepository repository)
    : IConsumer<ReservationConfirmedIntegrationEvent>
{
    public async Task Consume(ConsumeContext<ReservationConfirmedIntegrationEvent> context)
    {
        var message = context.Message;
        if (await repository.ReservationWasIssuedAsync(
                message.ReservationId,
                context.CancellationToken))
            return;

        var tickets = Enumerable.Range(0, message.Quantity)
            .Select(_ => AdmissionTicket.Issue(
                message.PaymentId,
                message.OrderId,
                message.ReservationId,
                message.UserId,
                message.EventId,
                message.TicketTypeId,
                message.TicketName,
                GenerateCode(),
                message.ConfirmedAt))
            .ToList();

        foreach (var ticket in tickets)
            ticket.CreatedBy = message.UserId;

        await repository.AddRangeAsync(tickets, context.CancellationToken);
        await context.Publish(
            new AdmissionTicketsIssuedIntegrationEvent(
                message.CorrelationId,
                message.PaymentId,
                message.OrderId,
                message.ReservationId,
                message.UserId,
                message.EventId,
                tickets.Count,
                message.ConfirmedAt,
                tickets.Select(ticket => new IssuedAdmissionTicket(
                    ticket.Id,
                    ticket.TicketName,
                    ticket.Code)).ToList()),
            publish => publish.CorrelationId = message.CorrelationId);
        await repository.SaveChangesAsync(context.CancellationToken);
    }

    private static string GenerateCode() =>
        Convert.ToBase64String(RandomNumberGenerator.GetBytes(32))
            .TrimEnd('=')
            .Replace('+', '-')
            .Replace('/', '_');
}
