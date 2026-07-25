namespace BuildingBlocks.Contracts.Orders;

public sealed record OrderCancelledIntegrationEvent(
    Guid CorrelationId,
    Guid OrderId,
    Guid ReservationId);
