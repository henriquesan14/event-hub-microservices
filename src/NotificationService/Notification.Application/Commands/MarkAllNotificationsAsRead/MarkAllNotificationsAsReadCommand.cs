using BuildingBlocks.SharedKernel.CQRS;
using BuildingBlocks.SharedKernel.Result;

namespace Notification.Application.Commands.MarkAllNotificationsAsRead;

public sealed record MarkAllNotificationsAsReadCommand : ICommand<ResultT<int>>;
