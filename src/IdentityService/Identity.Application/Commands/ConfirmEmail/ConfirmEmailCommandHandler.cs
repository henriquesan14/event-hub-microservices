using BuildingBlocks.SharedKernel.CQRS;
using BuildingBlocks.SharedKernel.Result;
using Identity.Application.Contracts;
using Identity.Application.Errors;
using Identity.Application.Security;

namespace Identity.Application.Commands.ConfirmEmail;

public sealed class ConfirmEmailCommandHandler(IUserRepository repository)
    : ICommandHandler<ConfirmEmailCommand, Result>
{
    public async Task<Result> Handle(ConfirmEmailCommand request, CancellationToken ct)
    {
        var hash = AccountTokenGenerator.Hash(request.Token);
        var user = await repository.GetByEmailConfirmationTokenHashAsync(hash, ct);
        if (user is null || !user.ConfirmEmail(hash, DateTime.Now))
            return AuthErrors.InvalidVerifiedToken();

        await repository.SaveChangesAsync(ct);
        return Result.Success();
    }
}
