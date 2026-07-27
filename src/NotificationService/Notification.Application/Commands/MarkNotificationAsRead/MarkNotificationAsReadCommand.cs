using BuildingBlocks.SharedKernel.CQRS;
using BuildingBlocks.SharedKernel.Result;

namespace Notification.Application.Commands.MarkNotificationAsRead;

public sealed record MarkNotificationAsReadCommand(Guid Id) : ICommand<Result>;
