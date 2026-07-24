using BuildingBlocks.SharedKernel.CQRS;
using BuildingBlocks.SharedKernel.Result;
using EventsApplication.Contracts;
using EventsApplication.Dtos;
using EventsApplication.Errors;
using EventsApplication.Extensions;

namespace EventsApplication.Queries.GetEventById;

public sealed class GetEventByIdQueryHandler(IEventRepository eventRepository)
    : IQueryHandler<GetEventByIdQuery, ResultT<EventDto>>
{
    public async Task<ResultT<EventDto>> Handle(GetEventByIdQuery request, CancellationToken ct)
    {
        var eventEntity = await eventRepository.GetByIdAsync(request.Id, ct);
        return eventEntity is null ? EventErrors.NotFound(request.Id) : eventEntity.ToDto();
    }
}
