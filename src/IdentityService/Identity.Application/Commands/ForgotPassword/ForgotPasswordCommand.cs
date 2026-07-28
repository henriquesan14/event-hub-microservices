using BuildingBlocks.SharedKernel.CQRS;
using BuildingBlocks.SharedKernel.Result;

namespace Identity.Application.Commands.ForgotPassword;

public sealed record ForgotPasswordCommand(string Email) : ICommand<Result>;
