using BuildingBlocks.SharedKernel.Result;

namespace Notification.Application.Errors;

public static class NotificationErrors
{
    public static Error NotFound(Guid id) =>
        Error.NotFound("Notification.NotFound", $"Notification with Id: {id} was not found");
    public static Error Unauthorized() =>
        Error.AccessUnAuthorized("Notification.Unauthorized", "Authentication is required");
    public static Error Forbidden() =>
        Error.AccessForbidden("Notification.Forbidden", "This notification belongs to another user");
}
