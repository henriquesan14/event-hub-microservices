using BuildingBlocks.SharedKernel.Abstractions;
using BuildingBlocks.SharedKernel.CQRS;
using BuildingBlocks.SharedKernel.Result;
using Identity.Application.Contracts;
using Identity.Application.Dtos;
using Identity.Application.Errors;
using Identity.Domain.Entities;
using Identity.Domain.ValueObjects;

namespace Identity.Application.Commands.RefreshAccessToken;

public sealed class RefreshAccessTokenCommandHandler(
    IUserRepository userRepository,
    ITokenService tokenService,
    IUserContext userContext)
    : ICommandHandler<RefreshAccessTokenCommand, ResultT<AuthResponse>>
{
    public async Task<ResultT<AuthResponse>> Handle(RefreshAccessTokenCommand request, CancellationToken ct)
    {
        if (string.IsNullOrWhiteSpace(userContext.RefreshToken))
            return AuthErrors.RefreshTokenNotFound();

        var currentToken = await userRepository.GetByRefreshTokenAsync(userContext.RefreshToken, ct);
        if (currentToken is null || currentToken.IsExpired || currentToken.IsRevoked)
            return AuthErrors.InvalidRefreshToken();

        var ipAddress = userContext.IpAddress ?? "unknown";
        var tokenResponse = tokenService.GenerateAccessToken(currentToken.User);
        var replacement = new RefreshToken(
            RefreshTokenId.New(),
            tokenResponse.RefreshToken,
            currentToken.UserId,
            ipAddress,
            tokenResponse.RefreshTokenExpiresAt);

        currentToken.Revoke(ipAddress);
        currentToken.SetReplacedByToken(replacement.Token);
        currentToken.User.AddRefreshToken(replacement);

        await userRepository.SaveChangesAsync(ct);
        userContext.SetCookieTokens(tokenResponse.AccessToken, tokenResponse.RefreshToken);

        return new AuthResponse(currentToken.User.Id.Value, currentToken.User.Name, currentToken.User.Role);
    }
}
