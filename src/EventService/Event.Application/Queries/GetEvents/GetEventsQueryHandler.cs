using BuildingBlocks.SharedKernel.CQRS;
using BuildingBlocks.SharedKernel.Result;
using EventsApplication.Contracts;
using EventsApplication.Dtos;
using EventsApplication.Extensions;

namespace Events.Application.Queries.GetEvents;

public record GetEventsQueryHandler(IEventRepository eventRepository) : IQueryHandler<GetEventsQuery, ResultT<PaginatedResult<EventDto>>>
{
    public async Task<ResultT<PaginatedResult<EventDto>>> Handle(GetEventsQuery request, CancellationToken ct)
    {
        var count = await eventRepository.CountAsync(request.Title, request.Status, ct);
        var events = await eventRepository.GetEvents(request.Title, request.Status, request.PageNumber, request.PageSize, ct);

        return new PaginatedResult<EventDto>(
            request.PageNumber,
            request.PageSize,
            count,
            events.ToDto());
    }
}
