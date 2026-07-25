using BuildingBlocks.SharedKernel.Abstractions;
using Ticketing.Domain.Enums;

namespace Ticketing.Domain.Entities;

public sealed class TicketReservation : AggregateRoot<Guid>
{
    private TicketReservation() { }

    private TicketReservation(Guid id, Guid ticketTypeId, Guid userId, int quantity, DateTime expiresAt)
    {
        Id = id;
        TicketTypeId = ticketTypeId;
        UserId = userId;
        Quantity = quantity;
        ExpiresAt = expiresAt;
        Status = ReservationStatus.Pending;
    }

    public Guid TicketTypeId { get; private set; }
    public Guid UserId { get; private set; }
    public int Quantity { get; private set; }
    public DateTime ExpiresAt { get; private set; }
    public ReservationStatus Status { get; private set; }
    public TicketType TicketType { get; private set; } = default!;

    public static TicketReservation Create(Guid ticketTypeId, Guid userId, int quantity, DateTime expiresAt)
    {
        if (ticketTypeId == Guid.Empty || userId == Guid.Empty)
            throw new DomainException("Ticket type and user are required.");
        if (quantity <= 0) throw new DomainException("Quantity must be greater than zero.");

        return new TicketReservation(Guid.NewGuid(), ticketTypeId, userId, quantity, expiresAt);
    }

    public void Confirm(DateTime now)
    {
        if (Status != ReservationStatus.Pending)
            throw new DomainException("Only pending reservations can be confirmed.");
        if (now >= ExpiresAt)
            throw new DomainException("Reservation has expired.");
        Status = ReservationStatus.Confirmed;
    }

    public void Release(bool expired)
    {
        if (Status != ReservationStatus.Pending)
            throw new DomainException("Only pending reservations can be released.");
        Status = expired ? ReservationStatus.Expired : ReservationStatus.Released;
    }
}
