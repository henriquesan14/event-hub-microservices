using Admission.Domain.Enums;
using BuildingBlocks.SharedKernel.Abstractions;

namespace Admission.Domain.Entities;

public sealed class AdmissionTicket : AggregateRoot<Guid>
{
    private AdmissionTicket() { }

    private AdmissionTicket(
        Guid id,
        Guid paymentId,
        Guid orderId,
        Guid reservationId,
        Guid userId,
        Guid eventId,
        Guid ticketTypeId,
        string ticketName,
        string code,
        DateTime issuedAt)
    {
        Id = id;
        PaymentId = paymentId;
        OrderId = orderId;
        ReservationId = reservationId;
        UserId = userId;
        EventId = eventId;
        TicketTypeId = ticketTypeId;
        TicketName = ticketName;
        Code = code;
        IssuedAt = issuedAt;
        Status = AdmissionTicketStatus.Active;
    }

    public Guid PaymentId { get; private set; }
    public Guid OrderId { get; private set; }
    public Guid ReservationId { get; private set; }
    public Guid UserId { get; private set; }
    public Guid EventId { get; private set; }
    public Guid TicketTypeId { get; private set; }
    public string TicketName { get; private set; } = string.Empty;
    public string Code { get; private set; } = string.Empty;
    public AdmissionTicketStatus Status { get; private set; }
    public DateTime IssuedAt { get; private set; }
    public DateTime? CheckedInAt { get; private set; }
    public Guid? CheckedInBy { get; private set; }
    public string? CheckInIpAddress { get; private set; }
    public DateTime? CancelledAt { get; private set; }

    public static AdmissionTicket Issue(
        Guid paymentId,
        Guid orderId,
        Guid reservationId,
        Guid userId,
        Guid eventId,
        Guid ticketTypeId,
        string ticketName,
        string code,
        DateTime issuedAt)
    {
        if (paymentId == Guid.Empty || orderId == Guid.Empty ||
            reservationId == Guid.Empty || userId == Guid.Empty ||
            eventId == Guid.Empty || ticketTypeId == Guid.Empty)
            throw new DomainException("Ticket references are required.");
        if (string.IsNullOrWhiteSpace(ticketName) || string.IsNullOrWhiteSpace(code))
            throw new DomainException("Ticket name and code are required.");

        return new AdmissionTicket(
            Guid.NewGuid(),
            paymentId,
            orderId,
            reservationId,
            userId,
            eventId,
            ticketTypeId,
            ticketName.Trim(),
            code.Trim(),
            issuedAt);
    }

    public void CheckIn(Guid operatorId, string ipAddress, DateTime now)
    {
        if (Status == AdmissionTicketStatus.Used)
            throw new DomainException("Ticket has already been used.");
        if (Status == AdmissionTicketStatus.Cancelled)
            throw new DomainException("Ticket is cancelled.");
        if (operatorId == Guid.Empty)
            throw new DomainException("Check-in operator is required.");

        Status = AdmissionTicketStatus.Used;
        CheckedInAt = now;
        CheckedInBy = operatorId;
        CheckInIpAddress = string.IsNullOrWhiteSpace(ipAddress)
            ? "unknown"
            : ipAddress.Trim();
    }

    public void Cancel(DateTime now)
    {
        if (Status != AdmissionTicketStatus.Active)
            return;
        Status = AdmissionTicketStatus.Cancelled;
        CancelledAt = now;
    }
}
