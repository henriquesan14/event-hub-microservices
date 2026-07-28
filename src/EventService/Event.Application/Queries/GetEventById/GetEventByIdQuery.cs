using BuildingBlocks.SharedKernel.CQRS;
using BuildingBlocks.SharedKernel.Result;
using EventsApplication.Dtos;

namespace EventsApplication.Queries.GetEventById;

public sealed record GetEventByIdQuery(
    Guid Id,
    Guid? UserId = null,
    bool CanManageOwn = false,
    bool CanManageAll = false) : IQuery<ResultT<EventDto>>;
