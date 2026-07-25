using BuildingBlocks.SharedKernel.Abstractions;
using Payment.Domain.Enums;

namespace Payment.Domain.Entities;

public sealed class Payment : AggregateRoot<Guid>
{
    private Payment() { }

    private Payment(
        Guid id,
        Guid orderId,
        Guid reservationId,
        Guid userId,
        decimal amount,
        string currency,
        DateTime expiresAt)
    {
        Id = id;
        OrderId = orderId;
        ReservationId = reservationId;
        UserId = userId;
        Amount = amount;
        Currency = currency;
        ExpiresAt = expiresAt;
        Status = PaymentStatus.Pending;
    }

    public Guid OrderId { get; private set; }
    public Guid ReservationId { get; private set; }
    public Guid UserId { get; private set; }
    public decimal Amount { get; private set; }
    public string Currency { get; private set; } = string.Empty;
    public PaymentStatus Status { get; private set; }
    public DateTime ExpiresAt { get; private set; }
    public DateTime? ApprovedAt { get; private set; }
    public DateTime? FailedAt { get; private set; }
    public string? ProviderReference { get; private set; }
    public string? FailureReason { get; private set; }

    public static Payment Create(
        Guid orderId,
        Guid reservationId,
        Guid userId,
        decimal amount,
        string currency,
        DateTime expiresAt)
    {
        if (orderId == Guid.Empty || reservationId == Guid.Empty || userId == Guid.Empty)
            throw new DomainException("Order, reservation and user are required.");
        if (amount < 0) throw new DomainException("Payment amount cannot be negative.");
        if (string.IsNullOrWhiteSpace(currency))
            throw new DomainException("Currency is required.");

        return new Payment(
            Guid.NewGuid(),
            orderId,
            reservationId,
            userId,
            amount,
            currency.Trim().ToUpperInvariant(),
            expiresAt);
    }

    public void Approve(string providerReference, DateTime now)
    {
        if (Status != PaymentStatus.Pending)
            throw new DomainException("Only pending payments can be approved.");
        if (now >= ExpiresAt)
            throw new DomainException("Payment has expired.");
        if (string.IsNullOrWhiteSpace(providerReference))
            throw new DomainException("Provider reference is required.");

        Status = PaymentStatus.Approved;
        ProviderReference = providerReference.Trim();
        ApprovedAt = now;
    }

    public void Fail(string reason, DateTime now)
    {
        if (Status != PaymentStatus.Pending)
            throw new DomainException("Only pending payments can fail.");
        if (string.IsNullOrWhiteSpace(reason))
            throw new DomainException("Failure reason is required.");

        Status = PaymentStatus.Failed;
        FailureReason = reason.Trim();
        FailedAt = now;
    }

    public void Cancel(DateTime now)
    {
        if (Status != PaymentStatus.Pending)
            return;

        Status = PaymentStatus.Cancelled;
        FailureReason = "Order cancelled";
        FailedAt = now;
    }

    public void Expire(DateTime now)
    {
        if (Status != PaymentStatus.Pending)
            return;

        Status = PaymentStatus.Expired;
        FailureReason = "Order expired";
        FailedAt = now;
    }
}
