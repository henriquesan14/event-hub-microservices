using BuildingBlocks.SharedKernel.CQRS;
using BuildingBlocks.SharedKernel.Result;

namespace Order.Application.Commands.CancelOrder;

public sealed record CancelOrderCommand(Guid Id) : ICommand<Result>;
