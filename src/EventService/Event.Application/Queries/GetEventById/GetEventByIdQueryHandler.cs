using BuildingBlocks.SharedKernel.CQRS;
using BuildingBlocks.SharedKernel.Result;
using EventsApplication.Contracts;
using EventsApplication.Dtos;
using EventsApplication.Errors;
using EventsApplication.Extensions;
using Events.Domain.Enums;

namespace EventsApplication.Queries.GetEventById;

public sealed class GetEventByIdQueryHandler(IEventRepository eventRepository)
    : IQueryHandler<GetEventByIdQuery, ResultT<EventDto>>
{
    public async Task<ResultT<EventDto>> Handle(GetEventByIdQuery request, CancellationToken ct)
    {
        var eventEntity = await eventRepository.GetByIdAsync(request.Id, ct);
        if (eventEntity is null)
            return EventErrors.NotFound(request.Id);

        var canView = eventEntity.Status == EventStatus.Published
            || request.CanManageAll
            || request.CanManageOwn
                && request.UserId is Guid userId
                && eventEntity.OrganizerId.Value == userId;

        return canView ? eventEntity.ToDto() : EventErrors.NotFound(request.Id);
    }
}
