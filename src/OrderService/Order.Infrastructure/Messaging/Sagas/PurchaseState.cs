using MassTransit;

namespace Order.Infrastructure.Messaging.Sagas;

public sealed class PurchaseState : SagaStateMachineInstance
{
    public Guid CorrelationId { get; set; }
    public string CurrentState { get; set; } = null!;
    public Guid ReservationId { get; set; }
    public Guid UserId { get; set; }
    public Guid? OrderId { get; set; }
    public Guid? PaymentId { get; set; }
    public Guid EventId { get; set; }
    public string EventName { get; set; } = null!;
    public DateTime? EventStartsAt { get; set; }
    public Guid TicketTypeId { get; set; }
    public int Quantity { get; set; }
    public decimal Total { get; set; }
    public string Currency { get; set; } = null!;
    public string? FailureReason { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime ExpiresAt { get; set; }
    public DateTime? OrderCreatedAt { get; set; }
    public DateTime? PaymentCompletedAt { get; set; }
    public DateTime? ReservationConfirmedAt { get; set; }
    public DateTime? TicketsIssuedAt { get; set; }
    public DateTime? CompletedAt { get; set; }
}
