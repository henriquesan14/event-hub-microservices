using BuildingBlocks.SharedKernel.Abstractions;
using BuildingBlocks.SharedKernel.CQRS;
using BuildingBlocks.SharedKernel.Result;
using Identity.Application.Contracts;
using Identity.Application.Dtos;
using Identity.Application.Errors;
using Identity.Domain.Contracts;
using Identity.Domain.Entities;
using Identity.Domain.ValueObjects;
using BuildingBlocks.Contracts.Users;
using MassTransit;

namespace Identity.Application.Commands.GenerateAccessToken;

public sealed class GenerateAccessTokenCommandHandler(IUserRepository userRepository, ITokenService tokenService,
    IPasswordCheck passwordCheck, IUserContext userContext, IPublishEndpoint publishEndpoint)
    : ICommandHandler<GenerateAccessTokenCommand, ResultT<AuthResponse>>
{
    public async Task<ResultT<AuthResponse>> Handle(GenerateAccessTokenCommand request, CancellationToken ct)
    {
        var user = await userRepository.GetByEmailAsync(request.Email, ct);
        if (user == null) return AuthErrors.Unauthorized();

        var canLogin = user.CanUserLogin(request.Password, passwordCheck);
        if (!canLogin) return AuthErrors.Unauthorized();
        if (!user.EmailConfirmed) return AuthErrors.EmailNotConfirmed();

        var tokenResponse = tokenService.GenerateAccessToken(user);

        var refreshToken = new RefreshToken(
            RefreshTokenId.New(),
            tokenResponse.RefreshToken,
            UserId.Of(user.Id.Value),
            userContext.IpAddress!,
            tokenResponse.RefreshTokenExpiresAt
        );

        user.AddRefreshToken(refreshToken);
        await publishEndpoint.Publish(
            new UserUpdatedIntegrationEvent(
                user.Id.Value,
                user.Id.Value,
                user.Name,
                user.Email.Value),
            context => context.CorrelationId = user.Id.Value,
            ct);

        await userRepository.SaveChangesAsync(ct);

        userContext.SetCookieTokens(tokenResponse.AccessToken, tokenResponse.RefreshToken);

        var authResponse = new AuthResponse(user.Id.Value, user.Name, user.Role);

        return authResponse;
    }
}
