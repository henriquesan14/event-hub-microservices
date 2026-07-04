using BuildingBlocks.SharedKernel.Abstractions;
using Events.Domain.ValueObjects;

namespace Events.Domain.Entities;

public sealed class Ticket : Entity<TicketId>
{

    private Ticket() { }
    private Ticket(TicketId id, string name, Money price, int quantity, int availableQuantity)
    {
        Id = id;
        Name = name;
        Price = price;
        Quantity = quantity;
        AvailableQuantity = availableQuantity;
    }

    public static Ticket Create(TicketId id, string name, Money price, int quantity, int availableQuantity)
    {
        return new Ticket(id, name, price, quantity, availableQuantity);
    }

    public string Name { get; private set; }

    public Money Price { get; private set; }

    public int Quantity { get; private set; }

    public int AvailableQuantity { get; private set; }
}
