namespace BuildingBlocks.Contracts.Tickets;

public sealed record ReservationConfirmedIntegrationEvent(
    Guid CorrelationId,
    Guid PaymentId,
    Guid OrderId,
    Guid ReservationId,
    Guid UserId,
    Guid EventId,
    string EventName,
    DateTime EventStartsAt,
    Guid TicketTypeId,
    string TicketName,
    int Quantity,
    DateTime ConfirmedAt);
