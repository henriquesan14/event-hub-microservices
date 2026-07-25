using Order.Domain.Enums;

namespace Order.Application.Dtos;

public sealed record OrderDto(
    Guid Id,
    Guid UserId,
    Guid ReservationId,
    OrderStatus Status,
    decimal Total,
    string Currency,
    DateTime ExpiresAt,
    Guid? PaymentId,
    IReadOnlyList<OrderItemDto> Items,
    DateTime? CreatedAt);

public sealed record OrderItemDto(
    Guid Id,
    Guid TicketTypeId,
    Guid EventId,
    string Name,
    decimal UnitPrice,
    string Currency,
    int Quantity,
    decimal Total);
