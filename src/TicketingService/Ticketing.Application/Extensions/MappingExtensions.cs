using Ticketing.Application.Dtos;
using Ticketing.Domain.Entities;

namespace Ticketing.Application.Extensions;

public static class MappingExtensions
{
    public static TicketTypeDto ToDto(this TicketType entity) =>
        new(entity.Id, entity.EventId, entity.Name, entity.Description, entity.Price,
            entity.Currency, entity.TotalQuantity, entity.AvailableQuantity,
            entity.SalesStart, entity.SalesEnd, entity.Status);

    public static ReservationDto ToDto(this TicketReservation entity) =>
        new(entity.Id, entity.TicketTypeId, entity.UserId, entity.Quantity, entity.ExpiresAt, entity.Status);
}
