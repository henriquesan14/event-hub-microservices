using BuildingBlocks.SharedKernel.CQRS;
using BuildingBlocks.SharedKernel.Result;

namespace Identity.Application.Commands.RequestEmailConfirmation;

public sealed record RequestEmailConfirmationCommand(string Email) : ICommand<Result>;
