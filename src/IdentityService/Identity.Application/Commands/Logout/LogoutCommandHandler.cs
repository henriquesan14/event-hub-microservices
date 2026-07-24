using BuildingBlocks.SharedKernel.Abstractions;
using BuildingBlocks.SharedKernel.CQRS;
using BuildingBlocks.SharedKernel.Result;
using Identity.Application.Contracts;

namespace Identity.Application.Commands.Logout;

public sealed class LogoutCommandHandler(IUserRepository userRepository, IUserContext userContext)
    : ICommandHandler<LogoutCommand, Result>
{
    public async Task<Result> Handle(LogoutCommand request, CancellationToken ct)
    {
        if (!string.IsNullOrWhiteSpace(userContext.RefreshToken))
        {
            var refreshToken = await userRepository.GetByRefreshTokenAsync(userContext.RefreshToken, ct);
            if (refreshToken is not null && !refreshToken.IsRevoked)
            {
                refreshToken.Revoke(userContext.IpAddress ?? "unknown");
                await userRepository.SaveChangesAsync(ct);
            }
        }

        userContext.RemoveCookiesToken();
        return Result.Success();
    }
}
