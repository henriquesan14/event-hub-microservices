namespace BuildingBlocks.Contracts.Ticketing;

public sealed record ReservationCreatedIntegrationEvent(
    Guid CorrelationId,
    Guid ReservationId,
    Guid UserId,
    string? UserName,
    Guid TicketTypeId,
    Guid EventId,
    string EventName,
    DateTime EventStartsAt,
    string TicketName,
    decimal UnitPrice,
    string Currency,
    int Quantity,
    DateTime ExpiresAt);
