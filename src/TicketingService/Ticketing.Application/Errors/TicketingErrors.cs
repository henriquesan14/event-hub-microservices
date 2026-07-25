using BuildingBlocks.SharedKernel.Result;

namespace Ticketing.Application.Errors;

public static class TicketingErrors
{
    public static Error TicketTypeNotFound(Guid id) =>
        Error.NotFound("Ticketing.TicketTypeNotFound", $"Ticket type with Id: {id} was not found");

    public static Error ReservationNotFound(Guid id) =>
        Error.NotFound("Ticketing.ReservationNotFound", $"Reservation with Id: {id} was not found");

    public static Error Unauthorized() =>
        Error.AccessUnAuthorized("Ticketing.Unauthorized", "Authentication is required");

    public static Error Forbidden() =>
        Error.AccessForbidden("Ticketing.Forbidden", "This reservation belongs to another user");

    public static Error TicketTypeForbidden() =>
        Error.AccessForbidden("Ticketing.TicketTypeForbidden", "Only the ticket type creator can modify it");
}
