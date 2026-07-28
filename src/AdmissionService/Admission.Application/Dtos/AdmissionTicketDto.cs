using Admission.Domain.Enums;

namespace Admission.Application.Dtos;

public sealed record AdmissionTicketDto(
    Guid Id,
    Guid OrderId,
    Guid ReservationId,
    Guid EventId,
    Guid TicketTypeId,
    string TicketName,
    string Code,
    AdmissionTicketStatus Status,
    DateTime IssuedAt,
    DateTime? CheckedInAt);
