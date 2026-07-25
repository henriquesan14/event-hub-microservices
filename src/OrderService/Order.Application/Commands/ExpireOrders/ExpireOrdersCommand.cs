using BuildingBlocks.SharedKernel.CQRS;
using BuildingBlocks.SharedKernel.Result;

namespace Order.Application.Commands.ExpireOrders;

public sealed record ExpireOrdersCommand : ICommand<ResultT<int>>;
