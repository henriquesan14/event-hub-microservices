namespace BuildingBlocks.Contracts.Payments;

public sealed record PaymentRefundedIntegrationEvent(
    Guid CorrelationId,
    Guid PaymentId,
    Guid OrderId,
    Guid ReservationId,
    Guid UserId,
    decimal Amount,
    string Currency,
    string? Reason,
    DateTime RefundedAt);
