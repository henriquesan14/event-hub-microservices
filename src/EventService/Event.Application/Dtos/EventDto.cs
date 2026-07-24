using Events.Domain.Enums;
using Events.Domain.ValueObjects;

namespace EventsApplication.Dtos;

public sealed record EventDto(
    Guid Id,
    string Title,
    string Description,
    Address Address,
    DateTime StartsAt,
    DateTime EndsAt,
    EventStatus Status,
    Guid OrganizerId,
    DateTime? CreatedAt,
    string? CreatedByName);
