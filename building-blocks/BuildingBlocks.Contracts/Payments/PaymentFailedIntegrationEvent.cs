namespace BuildingBlocks.Contracts.Payments;

public sealed record PaymentFailedIntegrationEvent(
    Guid CorrelationId,
    Guid PaymentId,
    Guid OrderId,
    Guid ReservationId,
    Guid UserId,
    string Reason,
    DateTime FailedAt);
