using BuildingBlocks.SharedKernel.CQRS;
using BuildingBlocks.SharedKernel.Result;

namespace Notification.Application.Queries.GetUnreadCount;

public sealed record GetUnreadCountQuery : IQuery<ResultT<int>>;
