using BuildingBlocks.SharedKernel.Abstractions;
using Events.Domain.Enums;
using Events.Domain.ValueObjects;

namespace Events.Domain.Entities;

public sealed class Event : AggregateRoot<EventId>
{
    private readonly List<Ticket> _tickets = [];

    private Event (){ }

    private Event(
        EventId id,
        string title,
        string description,
        Address address,
        DateTime startsAt,
        DateTime endsAt,
        UserId organizerId)
    {
        Id = id;
        Title = title;
        Description = description;
        Address = address;
        StartsAt = startsAt;
        EndsAt = endsAt;
        OrganizerId = organizerId;
        Status = EventStatus.Draft;
    }

    public string Title { get; private set; }

    public string Description { get; private set; }

    public Address Address { get; private set; }

    public DateTime StartsAt { get; private set; }

    public DateTime EndsAt { get; private set; }

    public EventStatus Status { get; private set; }

    public UserId OrganizerId { get; private set; }

    public IReadOnlyCollection<Ticket> Tickets => _tickets.AsReadOnly();

    public static Event Create(
        EventId eventId,
        string title,
        string description,
        Address address,
        DateTime startsAt,
        DateTime endsAt,
        UserId organizerId)
    {
        if (startsAt >= endsAt)
            throw new DomainException("The start date must be before the end date.");

        return new Event(
            eventId,
            title,
            description,
            address,
            startsAt,
            endsAt,
            organizerId);
    }

    public void Publish()
    {
        if (Status != EventStatus.Draft)
            throw new DomainException("Only draft events can be published.");

        if (StartsAt <= DateTime.UtcNow)
            throw new DomainException("Past events cannot be published.");

        Status = EventStatus.Published;
    }

    public void Cancel()
    {
        if (Status is EventStatus.Cancelled or EventStatus.Finished)
            throw new DomainException("Cancelled or finished events cannot be cancelled.");

        Status = EventStatus.Cancelled;
    }

    public void Update(
        string title,
        string description,
        Address address,
        DateTime startsAt,
        DateTime endsAt)
    {
        if (Status == EventStatus.Cancelled)
            throw new DomainException("Cancelled events cannot be updated.");

        if (startsAt >= endsAt)
            throw new DomainException("The start date must be before the end date.");

        Title = title;
        Description = description;
        Address = address;
        StartsAt = startsAt;
        EndsAt = endsAt;
    }

    public void AddTicket(Ticket ticket)
    {
        _tickets.Add(ticket);
    }
}
