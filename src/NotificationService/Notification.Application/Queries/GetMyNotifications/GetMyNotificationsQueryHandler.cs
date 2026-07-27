using BuildingBlocks.SharedKernel.Abstractions;
using BuildingBlocks.SharedKernel.CQRS;
using BuildingBlocks.SharedKernel.Result;
using Notification.Application.Contracts;
using Notification.Application.Dtos;
using Notification.Application.Errors;
using Notification.Application.Extensions;

namespace Notification.Application.Queries.GetMyNotifications;

public sealed class GetMyNotificationsQueryHandler(
    INotificationRepository repository,
    IUserContext userContext)
    : IQueryHandler<GetMyNotificationsQuery, ResultT<IReadOnlyList<NotificationDto>>>
{
    public async Task<ResultT<IReadOnlyList<NotificationDto>>> Handle(
        GetMyNotificationsQuery request,
        CancellationToken ct)
    {
        if (userContext.UserId is not Guid userId) return NotificationErrors.Unauthorized();
        var notifications = await repository.GetByUserAsync(userId, ct);
        return notifications.Select(x => x.ToDto()).ToList();
    }
}
