using BuildingBlocks.SharedKernel.CQRS;
using BuildingBlocks.SharedKernel.Result;

namespace Identity.Application.Commands.ChangePassword;

public sealed record ChangePasswordCommand(string CurrentPassword, string NewPassword) : ICommand<Result>;
