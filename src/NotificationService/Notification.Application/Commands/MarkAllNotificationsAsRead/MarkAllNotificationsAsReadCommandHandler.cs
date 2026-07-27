using BuildingBlocks.SharedKernel.Abstractions;
using BuildingBlocks.SharedKernel.CQRS;
using BuildingBlocks.SharedKernel.Result;
using Notification.Application.Contracts;
using Notification.Application.Errors;

namespace Notification.Application.Commands.MarkAllNotificationsAsRead;

public sealed class MarkAllNotificationsAsReadCommandHandler(
    INotificationRepository repository,
    IUserContext userContext)
    : ICommandHandler<MarkAllNotificationsAsReadCommand, ResultT<int>>
{
    public async Task<ResultT<int>> Handle(
        MarkAllNotificationsAsReadCommand request,
        CancellationToken ct)
    {
        if (userContext.UserId is not Guid userId) return NotificationErrors.Unauthorized();
        var notifications = await repository.GetUnreadByUserAsync(userId, ct);
        var now = DateTime.Now;
        foreach (var notification in notifications)
            notification.MarkAsRead(now);

        if (notifications.Count > 0)
            await repository.SaveChangesAsync(ct);
        return notifications.Count;
    }
}
