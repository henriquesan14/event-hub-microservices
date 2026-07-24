using BuildingBlocks.SharedKernel.CQRS;
using BuildingBlocks.SharedKernel.Result;

namespace EventsApplication.Commands.DeleteEvent;

public sealed record DeleteEventCommand(Guid Id) : ICommand<Result>;
