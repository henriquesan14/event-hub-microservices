using BuildingBlocks.SharedKernel.CQRS;
using BuildingBlocks.SharedKernel.Result;
using Events.Domain.Enums;
using EventsApplication.Dtos;

namespace Events.Application.Queries.GetEvents;

public sealed record GetEventsQuery(
    string? Title,
    EventStatus? Status,
    int PageNumber = 1,
    int PageSize = 20,
    Guid? OwnerId = null,
    bool IncludePublished = true,
    bool IncludeAll = false) : IQuery<ResultT<PaginatedResult<EventDto>>>;

