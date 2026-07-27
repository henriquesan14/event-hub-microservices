using BuildingBlocks.Api.Extensions;
using Carter;
using MediatR;
using Notification.Application.Commands.MarkAllNotificationsAsRead;
using Notification.Application.Commands.MarkNotificationAsRead;
using Notification.Application.Queries.GetMyNotifications;
using Notification.Application.Queries.GetUnreadCount;

namespace Notification.Api.Endpoints;

public sealed class NotificationModule : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/notifications").RequireAuthorization();
        group.MapGet("/me", GetMine);
        group.MapGet("/unread-count", GetUnreadCount);
        group.MapPost("/{id:guid}/read", MarkAsRead);
        group.MapPost("/read-all", MarkAllAsRead);
    }

    private static async Task<IResult> GetMine(ISender sender, CancellationToken ct) =>
        (await sender.Send(new GetMyNotificationsQuery(), ct)).ToHttpResult();

    private static async Task<IResult> GetUnreadCount(ISender sender, CancellationToken ct) =>
        (await sender.Send(new GetUnreadCountQuery(), ct)).ToHttpResult();

    private static async Task<IResult> MarkAsRead(
        Guid id,
        ISender sender,
        CancellationToken ct) =>
        (await sender.Send(new MarkNotificationAsReadCommand(id), ct)).ToHttpResult();

    private static async Task<IResult> MarkAllAsRead(ISender sender, CancellationToken ct) =>
        (await sender.Send(new MarkAllNotificationsAsReadCommand(), ct)).ToHttpResult();
}
