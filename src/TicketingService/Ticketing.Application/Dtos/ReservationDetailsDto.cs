using Ticketing.Domain.Enums;

namespace Ticketing.Application.Dtos;

public sealed record ReservationDetailsDto(
    Guid Id,
    Guid UserId,
    Guid TicketTypeId,
    Guid EventId,
    string TicketName,
    decimal UnitPrice,
    string Currency,
    int Quantity,
    DateTime ExpiresAt,
    ReservationStatus Status);
