using BuildingBlocks.Contracts.Orders;
using BuildingBlocks.Contracts.Payments;
using BuildingBlocks.Contracts.Ticketing;
using BuildingBlocks.Contracts.Tickets;
using MassTransit;

namespace Order.Infrastructure.Messaging.Sagas;

public sealed class PurchaseStateMachine : MassTransitStateMachine<PurchaseState>
{
    public State AwaitingOrder { get; private set; } = null!;
    public State AwaitingPayment { get; private set; } = null!;
    public State AwaitingReservationConfirmation { get; private set; } = null!;
    public State AwaitingTickets { get; private set; } = null!;
    public State Completed { get; private set; } = null!;
    public State PaymentFailed { get; private set; } = null!;
    public State Cancelled { get; private set; } = null!;
    public State Expired { get; private set; } = null!;
    public State Refunded { get; private set; } = null!;

    public Event<ReservationCreatedIntegrationEvent> ReservationCreated { get; private set; } = null!;
    public Event<OrderCreatedIntegrationEvent> OrderCreated { get; private set; } = null!;
    public Event<PaymentApprovedIntegrationEvent> PaymentApproved { get; private set; } = null!;
    public Event<PaymentFailedIntegrationEvent> PaymentRejected { get; private set; } = null!;
    public Event<ReservationConfirmedIntegrationEvent> ReservationConfirmed { get; private set; } = null!;
    public Event<AdmissionTicketsIssuedIntegrationEvent> TicketsIssued { get; private set; } = null!;
    public Event<OrderCancelledIntegrationEvent> OrderCancelled { get; private set; } = null!;
    public Event<OrderExpiredIntegrationEvent> OrderExpired { get; private set; } = null!;
    public Event<PaymentRefundedIntegrationEvent> PaymentRefunded { get; private set; } = null!;

    public PurchaseStateMachine()
    {
        InstanceState(x => x.CurrentState);

        Event(() => ReservationCreated, x => x.CorrelateById(context => context.Message.CorrelationId));
        Event(() => OrderCreated, x => x.CorrelateById(context => context.Message.CorrelationId));
        Event(() => PaymentApproved, x => x.CorrelateById(context => context.Message.CorrelationId));
        Event(() => PaymentRejected, x => x.CorrelateById(context => context.Message.CorrelationId));
        Event(() => ReservationConfirmed, x => x.CorrelateById(context => context.Message.CorrelationId));
        Event(() => TicketsIssued, x => x.CorrelateById(context => context.Message.CorrelationId));
        Event(() => OrderCancelled, x => x.CorrelateById(context => context.Message.CorrelationId));
        Event(() => OrderExpired, x => x.CorrelateById(context => context.Message.CorrelationId));
        Event(() => PaymentRefunded, x => x.CorrelateById(context => context.Message.CorrelationId));

        Initially(
            When(ReservationCreated)
                .Then(context =>
                {
                    var message = context.Message;
                    context.Saga.ReservationId = message.ReservationId;
                    context.Saga.UserId = message.UserId;
                    context.Saga.EventId = message.EventId;
                    context.Saga.EventName = message.EventName;
                    context.Saga.EventStartsAt = message.EventStartsAt;
                    context.Saga.TicketTypeId = message.TicketTypeId;
                    context.Saga.Quantity = message.Quantity;
                    context.Saga.Total = message.UnitPrice * message.Quantity;
                    context.Saga.Currency = message.Currency;
                    context.Saga.CreatedAt = DateTime.Now;
                    context.Saga.ExpiresAt = message.ExpiresAt;
                })
                .TransitionTo(AwaitingOrder));

        During(AwaitingOrder,
            When(OrderCreated)
                .Then(context =>
                {
                    context.Saga.OrderId = context.Message.OrderId;
                    context.Saga.Total = context.Message.Total;
                    context.Saga.Currency = context.Message.Currency;
                    context.Saga.OrderCreatedAt = context.Message.CreatedAt;
                    context.Saga.ExpiresAt = context.Message.ExpiresAt;
                })
                .TransitionTo(AwaitingPayment));

        During(AwaitingPayment,
            When(PaymentApproved)
                .Then(context =>
                {
                    context.Saga.PaymentId = context.Message.PaymentId;
                    context.Saga.PaymentCompletedAt = context.Message.ApprovedAt;
                })
                .TransitionTo(AwaitingReservationConfirmation),
            When(PaymentRejected)
                .Then(context =>
                {
                    context.Saga.PaymentId = context.Message.PaymentId;
                    context.Saga.PaymentCompletedAt = context.Message.FailedAt;
                    context.Saga.FailureReason = context.Message.Reason;
                    context.Saga.CompletedAt = context.Message.FailedAt;
                })
                .TransitionTo(PaymentFailed));

        During(AwaitingReservationConfirmation,
            When(ReservationConfirmed)
                .Then(context => context.Saga.ReservationConfirmedAt = context.Message.ConfirmedAt)
                .TransitionTo(AwaitingTickets));

        During(AwaitingTickets,
            When(TicketsIssued)
                .Then(context =>
                {
                    context.Saga.TicketsIssuedAt = context.Message.IssuedAt;
                    context.Saga.CompletedAt = context.Message.IssuedAt;
                })
                .TransitionTo(Completed));

        DuringAny(
            When(OrderCancelled)
                .Then(context => context.Saga.CompletedAt = DateTime.Now)
                .TransitionTo(Cancelled),
            When(OrderExpired)
                .Then(context => context.Saga.CompletedAt = DateTime.Now)
                .TransitionTo(Expired),
            When(PaymentRefunded)
                .Then(context =>
                {
                    context.Saga.PaymentId = context.Message.PaymentId;
                    context.Saga.CompletedAt = context.Message.RefundedAt;
                    context.Saga.FailureReason = context.Message.Reason;
                })
                .TransitionTo(Refunded));
    }
}
