using BuildingBlocks.SharedKernel.Abstractions;

namespace Order.Domain.Entities;

public sealed class OrderItem : Entity<Guid>
{
    private OrderItem() { }

    private OrderItem(
        Guid id,
        Guid orderId,
        Guid ticketTypeId,
        Guid eventId,
        string eventName,
        DateTime? eventStartsAt,
        string name,
        decimal unitPrice,
        string currency,
        int quantity)
    {
        Id = id;
        OrderId = orderId;
        TicketTypeId = ticketTypeId;
        EventId = eventId;
        EventName = eventName.Trim();
        EventStartsAt = eventStartsAt;
        Name = name;
        UnitPrice = unitPrice;
        Currency = currency.ToUpperInvariant();
        Quantity = quantity;
        Total = unitPrice * quantity;
    }

    public Guid OrderId { get; private set; }
    public Guid TicketTypeId { get; private set; }
    public Guid EventId { get; private set; }
    public string EventName { get; private set; } = string.Empty;
    public DateTime? EventStartsAt { get; private set; }
    public string Name { get; private set; } = string.Empty;
    public decimal UnitPrice { get; private set; }
    public string Currency { get; private set; } = string.Empty;
    public int Quantity { get; private set; }
    public decimal Total { get; private set; }

    internal static OrderItem Create(
        Guid orderId,
        Guid ticketTypeId,
        Guid eventId,
        string eventName,
        DateTime eventStartsAt,
        string name,
        decimal unitPrice,
        string currency,
        int quantity) =>
        new(Guid.NewGuid(), orderId, ticketTypeId, eventId, eventName, eventStartsAt,
            name, unitPrice, currency, quantity);
}
