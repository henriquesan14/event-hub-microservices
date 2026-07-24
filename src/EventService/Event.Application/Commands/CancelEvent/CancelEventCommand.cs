using BuildingBlocks.SharedKernel.CQRS;
using BuildingBlocks.SharedKernel.Result;
using EventsApplication.Dtos;

namespace EventsApplication.Commands.CancelEvent;

public sealed record CancelEventCommand(Guid Id) : ICommand<ResultT<EventDto>>;
