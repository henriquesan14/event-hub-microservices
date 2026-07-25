using BuildingBlocks.SharedKernel.Result;

namespace Order.Application.Errors;

public static class OrderErrors
{
    public static Error NotFound(Guid id) =>
        Error.NotFound("Order.NotFound", $"Order with Id: {id} was not found");
    public static Error Unauthorized() =>
        Error.AccessUnAuthorized("Order.Unauthorized", "Authentication is required");
    public static Error Forbidden() =>
        Error.AccessForbidden("Order.Forbidden", "This order belongs to another user");
    public static Error ReservationInvalid() =>
        Error.Validation("Order.InvalidReservation", "Reservation was not found, is not pending, or has expired");
    public static Error ReservationAlreadyUsed() =>
        Error.Conflict("Order.ReservationAlreadyUsed", "An order already exists for this reservation");
    public static Error TicketingUnavailable() =>
        Error.Failure("Order.TicketingUnavailable", "Ticketing service could not release the reservation");
}
