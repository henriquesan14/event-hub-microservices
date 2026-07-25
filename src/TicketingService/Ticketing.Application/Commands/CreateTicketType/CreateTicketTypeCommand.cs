using BuildingBlocks.SharedKernel.CQRS;
using BuildingBlocks.SharedKernel.Result;
using Ticketing.Application.Dtos;

namespace Ticketing.Application.Commands.CreateTicketType;

public sealed record CreateTicketTypeCommand(
    Guid EventId,
    string Name,
    string Description,
    decimal Price,
    string Currency,
    int TotalQuantity,
    DateTime SalesStart,
    DateTime SalesEnd) : ICommand<ResultT<TicketTypeDto>>;
