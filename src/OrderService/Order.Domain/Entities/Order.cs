using BuildingBlocks.SharedKernel.Abstractions;
using Order.Domain.Enums;

namespace Order.Domain.Entities;

public sealed class Order : AggregateRoot<Guid>
{
    private readonly List<OrderItem> _items = [];

    private Order() { }

    private Order(Guid id, Guid userId, Guid reservationId, DateTime expiresAt, OrderItem item)
    {
        Id = id;
        UserId = userId;
        ReservationId = reservationId;
        ExpiresAt = expiresAt;
        Status = OrderStatus.PendingPayment;
        Currency = item.Currency;
        Total = item.Total;
        _items.Add(item);
    }

    public Guid UserId { get; private set; }
    public Guid ReservationId { get; private set; }
    public OrderStatus Status { get; private set; }
    public decimal Total { get; private set; }
    public string Currency { get; private set; } = string.Empty;
    public DateTime ExpiresAt { get; private set; }
    public Guid? PaymentId { get; private set; }
    public IReadOnlyCollection<OrderItem> Items => _items.AsReadOnly();

    public static Order Create(
        Guid userId,
        Guid reservationId,
        Guid ticketTypeId,
        Guid eventId,
        string eventName,
        DateTime eventStartsAt,
        string ticketName,
        decimal unitPrice,
        string currency,
        int quantity,
        DateTime expiresAt)
    {
        if (userId == Guid.Empty || reservationId == Guid.Empty)
            throw new DomainException("User and reservation are required.");
        if (quantity <= 0) throw new DomainException("Quantity must be greater than zero.");
        if (unitPrice < 0) throw new DomainException("Unit price cannot be negative.");
        if (expiresAt <= DateTime.Now) throw new DomainException("Reservation has expired.");

        var orderId = Guid.NewGuid();
        var item = OrderItem.Create(
            orderId, ticketTypeId, eventId, eventName, eventStartsAt,
            ticketName, unitPrice, currency, quantity);
        return new Order(orderId, userId, reservationId, expiresAt, item);
    }

    public void Cancel()
    {
        if (Status != OrderStatus.PendingPayment)
            throw new DomainException("Only pending orders can be cancelled.");
        Status = OrderStatus.Cancelled;
    }

    public void Expire(DateTime now)
    {
        if (Status != OrderStatus.PendingPayment)
            throw new DomainException("Only pending orders can expire.");
        if (now < ExpiresAt)
            throw new DomainException("Order has not expired yet.");
        Status = OrderStatus.Expired;
    }

    public void ConfirmPayment(Guid paymentId)
    {
        if (Status != OrderStatus.PendingPayment)
            throw new DomainException("Only pending orders can be paid.");
        PaymentId = paymentId;
        Status = OrderStatus.Paid;
    }

    public void FailPayment(Guid paymentId)
    {
        if (Status != OrderStatus.PendingPayment)
            throw new DomainException("Only pending orders can fail payment.");
        PaymentId = paymentId;
        Status = OrderStatus.PaymentFailed;
    }

    public void Refund(Guid paymentId)
    {
        if (Status == OrderStatus.Refunded)
            return;
        if (Status != OrderStatus.Paid || PaymentId != paymentId)
            throw new DomainException("Only the paid order can be refunded.");
        Status = OrderStatus.Refunded;
    }
}
