using BuildingBlocks.SharedKernel.CQRS;
using BuildingBlocks.SharedKernel.Result;
using Identity.Application.Contracts;
using Identity.Application.Errors;
using Identity.Application.Security;
using Identity.Domain.Contracts;

namespace Identity.Application.Commands.ResetPassword;

public sealed class ResetPasswordCommandHandler(
    IUserRepository repository,
    IPasswordHash passwordHash)
    : ICommandHandler<ResetPasswordCommand, Result>
{
    public async Task<Result> Handle(ResetPasswordCommand request, CancellationToken ct)
    {
        var hash = AccountTokenGenerator.Hash(request.Token);
        var user = await repository.GetByPasswordResetTokenHashAsync(hash, ct);
        if (user is null ||
            !user.ResetPassword(hash, request.NewPassword, DateTime.Now, passwordHash))
            return AuthErrors.InvalidVerifiedToken();

        await repository.SaveChangesAsync(ct);
        return Result.Success();
    }
}
