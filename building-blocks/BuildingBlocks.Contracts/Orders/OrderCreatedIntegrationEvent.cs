namespace BuildingBlocks.Contracts.Orders;

public sealed record OrderCreatedIntegrationEvent(
    Guid CorrelationId,
    Guid OrderId,
    Guid ReservationId,
    Guid UserId,
    decimal Total,
    string Currency,
    DateTime ExpiresAt);
