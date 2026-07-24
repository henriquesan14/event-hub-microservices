using Events.Domain.Entities;
using EventsApplication.Dtos;

namespace EventsApplication.Extensions;

public static class EventExtensions
{
    public static EventDto ToDto(this Event eventEntity)
    {
        return new EventDto(
            eventEntity.Id.Value,
            eventEntity.Title,
            eventEntity.Description,
            eventEntity.Address,
            eventEntity.StartsAt,
            eventEntity.EndsAt,
            eventEntity.Status,
            eventEntity.OrganizerId.Value,
            eventEntity.CreatedAt,
            eventEntity.CreatedByName
        );
    }

    public static List<EventDto> ToDto(this IEnumerable<Event> events)
    {
        return events
            .Select(ToDto)
            .ToList();
    }
}
