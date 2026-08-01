using Admission.Application.Dtos;
using Admission.Domain.Entities;

namespace Admission.Application.Extensions;

public static class AdmissionTicketExtensions
{
    public static AdmissionTicketDto ToDto(this AdmissionTicket ticket) =>
        new(
            ticket.Id,
            ticket.OrderId,
            ticket.ReservationId,
            ticket.EventId,
            ticket.EventName,
            ticket.EventStartsAt,
            ticket.TicketTypeId,
            ticket.TicketName,
            ticket.Code,
            ticket.Status,
            ticket.IssuedAt,
            ticket.CheckedInAt);
}
