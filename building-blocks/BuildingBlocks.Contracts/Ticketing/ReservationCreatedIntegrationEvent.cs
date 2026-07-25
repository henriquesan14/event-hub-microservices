namespace BuildingBlocks.Contracts.Ticketing;

public sealed record ReservationCreatedIntegrationEvent(
    Guid CorrelationId,
    Guid ReservationId,
    Guid UserId,
    string? UserName,
    Guid TicketTypeId,
    Guid EventId,
    string TicketName,
    decimal UnitPrice,
    string Currency,
    int Quantity,
    DateTime ExpiresAt);
