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
        string name,
        decimal unitPrice,
        string currency,
        int quantity)
    {
        Id = id;
        OrderId = orderId;
        TicketTypeId = ticketTypeId;
        EventId = eventId;
        Name = name;
        UnitPrice = unitPrice;
        Currency = currency.ToUpperInvariant();
        Quantity = quantity;
        Total = unitPrice * quantity;
    }

    public Guid OrderId { get; private set; }
    public Guid TicketTypeId { get; private set; }
    public Guid EventId { get; private set; }
    public string Name { get; private set; } = string.Empty;
    public decimal UnitPrice { get; private set; }
    public string Currency { get; private set; } = string.Empty;
    public int Quantity { get; private set; }
    public decimal Total { get; private set; }

    internal static OrderItem Create(
        Guid orderId,
        Guid ticketTypeId,
        Guid eventId,
        string name,
        decimal unitPrice,
        string currency,
        int quantity) =>
        new(Guid.NewGuid(), orderId, ticketTypeId, eventId, name, unitPrice, currency, quantity);
}
