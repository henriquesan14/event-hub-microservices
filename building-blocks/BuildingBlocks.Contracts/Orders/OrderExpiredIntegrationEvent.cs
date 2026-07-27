namespace BuildingBlocks.Contracts.Orders;

public sealed record OrderExpiredIntegrationEvent(
    Guid CorrelationId,
    Guid OrderId,
    Guid ReservationId,
    Guid UserId);
