using Ticketing.Domain.Enums;

namespace Ticketing.Application.Dtos;

public sealed record TicketTypeDto(
    Guid Id,
    Guid EventId,
    string EventName,
    DateTime? EventStartsAt,
    string Name,
    string Description,
    decimal Price,
    string Currency,
    int TotalQuantity,
    int AvailableQuantity,
    DateTime SalesStart,
    DateTime SalesEnd,
    TicketTypeStatus Status);
