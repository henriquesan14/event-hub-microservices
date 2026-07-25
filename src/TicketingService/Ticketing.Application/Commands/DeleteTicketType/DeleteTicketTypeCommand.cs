using BuildingBlocks.SharedKernel.CQRS;
using BuildingBlocks.SharedKernel.Result;

namespace Ticketing.Application.Commands.DeleteTicketType;

public sealed record DeleteTicketTypeCommand(Guid Id) : ICommand<Result>;
