using BuildingBlocks.SharedKernel.Abstractions;
using BuildingBlocks.SharedKernel.CQRS;
using BuildingBlocks.SharedKernel.Result;
using Identity.Application.Contracts;
using Identity.Application.Errors;
using Identity.Domain.Contracts;

namespace Identity.Application.Commands.ChangePassword;

public sealed class ChangePasswordCommandHandler(
    IUserRepository userRepository,
    IUserContext userContext,
    IPasswordCheck passwordCheck,
    IPasswordHash passwordHash)
    : ICommandHandler<ChangePasswordCommand, Result>
{
    public async Task<Result> Handle(ChangePasswordCommand request, CancellationToken ct)
    {
        if (userContext.UserId is not Guid userId)
            return AuthErrors.SessionExpired();

        var user = await userRepository.GetByIdAsync(userId, ct);
        if (user is null)
            return UserErrors.NotFound(userId);

        if (!user.ChangePassword(request.CurrentPassword, request.NewPassword, passwordCheck, passwordHash))
            return AuthErrors.InvalidCurrentPassword();

        await userRepository.SaveChangesAsync(ct);
        return Result.Success();
    }
}
