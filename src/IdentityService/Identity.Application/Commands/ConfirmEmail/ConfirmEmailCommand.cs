using BuildingBlocks.SharedKernel.CQRS;
using BuildingBlocks.SharedKernel.Result;

namespace Identity.Application.Commands.ConfirmEmail;

public sealed record ConfirmEmailCommand(string Token) : ICommand<Result>;
