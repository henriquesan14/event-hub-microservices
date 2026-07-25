using Ticketing.Domain.Enums;

namespace Ticketing.Application.Dtos;

public sealed record ReservationDto(
    Guid Id,
    Guid TicketTypeId,
    Guid UserId,
    int Quantity,
    DateTime ExpiresAt,
    ReservationStatus Status);
