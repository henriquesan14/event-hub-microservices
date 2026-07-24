using BuildingBlocks.SharedKernel.Result;

namespace EventsApplication.Errors;

public static class EventErrors
{
    public static Error NotFound(Guid id) =>
        Error.NotFound("Event.NotFound", $"Event with Id: {id} was not found");

    public static Error Forbidden() =>
        Error.AccessForbidden("Event.Forbidden", "Only the event organizer can perform this action");

    public static Error Unauthorized() =>
        Error.AccessUnAuthorized("Event.Unauthorized", "Authentication is required");
}
