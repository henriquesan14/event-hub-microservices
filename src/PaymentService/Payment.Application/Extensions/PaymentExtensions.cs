using Payment.Application.Dtos;

namespace Payment.Application.Extensions;

public static class PaymentExtensions
{
    public static PaymentDto ToDto(this Domain.Entities.Payment payment) =>
        new(
            payment.Id,
            payment.OrderId,
            payment.ReservationId,
            payment.UserId,
            payment.Amount,
            payment.Currency,
            payment.Status,
            payment.ExpiresAt,
            payment.ApprovedAt,
            payment.FailedAt,
            payment.ProviderReference,
            payment.ProviderCustomerReference,
            payment.BillingType,
            payment.InvoiceUrl,
            payment.FailureReason,
            payment.CreatedAt,
            payment.RefundRequestedAt,
            payment.RefundedAt,
            payment.RefundReason);
}
