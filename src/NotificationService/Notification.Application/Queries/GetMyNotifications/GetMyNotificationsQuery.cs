using BuildingBlocks.SharedKernel.CQRS;
using BuildingBlocks.SharedKernel.Result;
using Notification.Application.Dtos;

namespace Notification.Application.Queries.GetMyNotifications;

public sealed record GetMyNotificationsQuery
    : IQuery<ResultT<IReadOnlyList<NotificationDto>>>;
