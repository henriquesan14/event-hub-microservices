using BuildingBlocks.SharedKernel.Abstractions;
using BuildingBlocks.SharedKernel.CQRS;
using BuildingBlocks.SharedKernel.Result;
using Notification.Application.Contracts;
using Notification.Application.Errors;

namespace Notification.Application.Queries.GetUnreadCount;

public sealed class GetUnreadCountQueryHandler(
    INotificationRepository repository,
    IUserContext userContext)
    : IQueryHandler<GetUnreadCountQuery, ResultT<int>>
{
    public async Task<ResultT<int>> Handle(GetUnreadCountQuery request, CancellationToken ct)
    {
        if (userContext.UserId is not Guid userId) return NotificationErrors.Unauthorized();
        return await repository.CountUnreadAsync(userId, ct);
    }
}
