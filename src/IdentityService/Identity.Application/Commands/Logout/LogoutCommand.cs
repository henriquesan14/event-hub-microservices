using BuildingBlocks.SharedKernel.CQRS;
using BuildingBlocks.SharedKernel.Result;

namespace Identity.Application.Commands.Logout;

public sealed record LogoutCommand : ICommand<Result>;
