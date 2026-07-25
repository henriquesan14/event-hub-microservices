using Payment.Domain.Enums;

namespace Payment.Application.Dtos;

public sealed record PaymentDto(
    Guid Id,
    Guid OrderId,
    Guid ReservationId,
    Guid UserId,
    decimal Amount,
    string Currency,
    PaymentStatus Status,
    DateTime ExpiresAt,
    DateTime? ApprovedAt,
    DateTime? FailedAt,
    string? ProviderReference,
    string? FailureReason,
    DateTime? CreatedAt);
