using BuildingBlocks.SharedKernel.CQRS;
using BuildingBlocks.SharedKernel.Result;
using Ticketing.Application.Dtos;

namespace Ticketing.Application.Commands.UpdateTicketType;

public sealed record UpdateTicketTypeCommand(
    Guid Id,
    string Name,
    string Description,
    decimal Price,
    string Currency,
    int TotalQuantity,
    DateTime SalesStart,
    DateTime SalesEnd,
    bool Active) : ICommand<ResultT<TicketTypeDto>>;
