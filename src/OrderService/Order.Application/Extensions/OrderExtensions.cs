using Order.Application.Dtos;

namespace Order.Application.Extensions;

public static class OrderExtensions
{
    public static OrderDto ToDto(this Domain.Entities.Order order) =>
        new(
            order.Id,
            order.UserId,
            order.ReservationId,
            order.Status,
            order.Total,
            order.Currency,
            order.ExpiresAt,
            order.PaymentId,
            order.Items.Select(x => new OrderItemDto(
                x.Id, x.TicketTypeId, x.EventId, x.EventName, x.EventStartsAt,
                x.Name, x.UnitPrice,
                x.Currency, x.Quantity, x.Total)).ToList(),
            order.CreatedAt);
}
