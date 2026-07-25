namespace BuildingBlocks.Contracts.Payments;

public sealed record PaymentApprovedIntegrationEvent(
    Guid CorrelationId,
    Guid PaymentId,
    Guid OrderId,
    Guid ReservationId,
    Guid UserId,
    decimal Amount,
    string Currency,
    DateTime ApprovedAt);
