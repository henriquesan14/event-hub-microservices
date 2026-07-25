namespace Order.Domain.Enums;

public enum OrderStatus
{
    PendingPayment,
    Paid,
    PaymentFailed,
    Cancelled,
    Expired,
    Refunded
}
