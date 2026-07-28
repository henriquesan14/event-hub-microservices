using BuildingBlocks.SharedKernel.CQRS;
using BuildingBlocks.SharedKernel.Result;

namespace Identity.Application.Commands.ResetPassword;

public sealed record ResetPasswordCommand(string Token, string NewPassword) : ICommand<Result>;
