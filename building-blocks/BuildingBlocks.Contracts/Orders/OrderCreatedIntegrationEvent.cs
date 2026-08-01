namespace BuildingBlocks.Contracts.Orders;

public sealed record OrderCreatedIntegrationEvent(
    Guid CorrelationId,
    Guid OrderId,
    Guid ReservationId,
    Guid UserId,
    string EventName,
    DateTime EventStartsAt,
    decimal Total,
    string Currency,
    DateTime CreatedAt,
    DateTime ExpiresAt);
