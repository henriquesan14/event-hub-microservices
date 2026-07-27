using BuildingBlocks.SharedKernel.Abstractions;
using BuildingBlocks.SharedKernel.CQRS;
using BuildingBlocks.SharedKernel.Result;
using Notification.Application.Contracts;
using Notification.Application.Errors;

namespace Notification.Application.Commands.MarkNotificationAsRead;

public sealed class MarkNotificationAsReadCommandHandler(
    INotificationRepository repository,
    IUserContext userContext)
    : ICommandHandler<MarkNotificationAsReadCommand, Result>
{
    public async Task<Result> Handle(MarkNotificationAsReadCommand request, CancellationToken ct)
    {
        if (userContext.UserId is not Guid userId) return NotificationErrors.Unauthorized();
        var notification = await repository.GetByIdAsync(request.Id, ct);
        if (notification is null) return NotificationErrors.NotFound(request.Id);
        if (notification.UserId != userId) return NotificationErrors.Forbidden();

        notification.MarkAsRead(DateTime.Now);
        await repository.SaveChangesAsync(ct);
        return Result.Success();
    }
}
