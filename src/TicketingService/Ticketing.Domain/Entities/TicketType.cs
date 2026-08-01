using BuildingBlocks.SharedKernel.Abstractions;
using Ticketing.Domain.Enums;

namespace Ticketing.Domain.Entities;

public sealed class TicketType : AggregateRoot<Guid>
{
    private TicketType() { }

    private TicketType(
        Guid id,
        Guid eventId,
        string eventName,
        DateTime eventStartsAt,
        string name,
        string description,
        decimal price,
        string currency,
        int totalQuantity,
        DateTime salesStart,
        DateTime salesEnd)
    {
        Id = id;
        EventId = eventId;
        EventName = eventName.Trim();
        EventStartsAt = eventStartsAt;
        Name = name;
        Description = description;
        Price = price;
        Currency = currency.ToUpperInvariant();
        TotalQuantity = totalQuantity;
        AvailableQuantity = totalQuantity;
        SalesStart = salesStart;
        SalesEnd = salesEnd;
        Status = TicketTypeStatus.Active;
        Version = 1;
    }

    public Guid EventId { get; private set; }
    public string EventName { get; private set; } = string.Empty;
    public DateTime? EventStartsAt { get; private set; }
    public string Name { get; private set; } = string.Empty;
    public string Description { get; private set; } = string.Empty;
    public decimal Price { get; private set; }
    public string Currency { get; private set; } = string.Empty;
    public int TotalQuantity { get; private set; }
    public int AvailableQuantity { get; private set; }
    public DateTime SalesStart { get; private set; }
    public DateTime SalesEnd { get; private set; }
    public TicketTypeStatus Status { get; private set; }
    public int Version { get; private set; }

    public static TicketType Create(
        Guid eventId,
        string eventName,
        DateTime eventStartsAt,
        string name,
        string description,
        decimal price,
        string currency,
        int totalQuantity,
        DateTime salesStart,
        DateTime salesEnd)
    {
        if (eventId == Guid.Empty) throw new DomainException("EventId is required.");
        if (string.IsNullOrWhiteSpace(eventName)) throw new DomainException("Event name is required.");
        if (eventStartsAt == default) throw new DomainException("Event start date is required.");
        if (price < 0) throw new DomainException("Price cannot be negative.");
        if (totalQuantity <= 0) throw new DomainException("Total quantity must be greater than zero.");
        if (salesStart >= salesEnd) throw new DomainException("Sales start must be before sales end.");

        return new TicketType(
            Guid.NewGuid(), eventId, eventName, eventStartsAt, name, description, price, currency,
            totalQuantity, salesStart, salesEnd);
    }

    public void Update(
        string name,
        string description,
        decimal price,
        string currency,
        int totalQuantity,
        DateTime salesStart,
        DateTime salesEnd,
        bool active)
    {
        var reservedOrSold = TotalQuantity - AvailableQuantity;
        if (price < 0) throw new DomainException("Price cannot be negative.");
        if (totalQuantity < reservedOrSold)
            throw new DomainException("Total quantity cannot be lower than reserved or sold quantity.");
        if (salesStart >= salesEnd) throw new DomainException("Sales start must be before sales end.");

        AvailableQuantity += totalQuantity - TotalQuantity;
        TotalQuantity = totalQuantity;
        Name = name;
        Description = description;
        Price = price;
        Currency = currency.ToUpperInvariant();
        SalesStart = salesStart;
        SalesEnd = salesEnd;
        Status = active
            ? AvailableQuantity == 0 ? TicketTypeStatus.SoldOut : TicketTypeStatus.Active
            : TicketTypeStatus.Inactive;
        Version++;
    }

    public void Reserve(int quantity, DateTime now)
    {
        if (Status != TicketTypeStatus.Active)
            throw new DomainException("Ticket type is not active.");
        if (now < SalesStart || now > SalesEnd)
            throw new DomainException("Tickets are outside the sales period.");
        if (quantity <= 0 || quantity > AvailableQuantity)
            throw new DomainException("Requested quantity is not available.");

        AvailableQuantity -= quantity;
        if (AvailableQuantity == 0) Status = TicketTypeStatus.SoldOut;
        Version++;
    }

    public void EnsureEventSnapshot(string eventName, DateTime eventStartsAt)
    {
        if (string.IsNullOrWhiteSpace(EventName))
        {
            if (string.IsNullOrWhiteSpace(eventName))
                throw new DomainException("Event name is required.");
            EventName = eventName.Trim();
        }
        if (EventStartsAt is null)
        {
            if (eventStartsAt == default)
                throw new DomainException("Event start date is required.");
            EventStartsAt = eventStartsAt;
        }
    }

    public void Release(int quantity)
    {
        if (quantity <= 0) throw new DomainException("Quantity must be greater than zero.");
        AvailableQuantity = Math.Min(TotalQuantity, AvailableQuantity + quantity);
        if (Status == TicketTypeStatus.SoldOut) Status = TicketTypeStatus.Active;
        Version++;
    }

}
