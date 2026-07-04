using BuildingBlocks.SharedKernel.CQRS;
using BuildingBlocks.SharedKernel.Result;
using Events.Domain.ValueObjects;
using EventsApplication.Dtos;

namespace EventsApplication.Commands.CreateEvent;

public sealed record CreateEventCommand(string Title, string Description, Address Address, DateTime StartsAt,
    DateTime EndsAt) : ICommand<ResultT<EventDto>>;


