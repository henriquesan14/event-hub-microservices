using BuildingBlocks.Contracts.Users;
using BuildingBlocks.SharedKernel.CQRS;
using BuildingBlocks.SharedKernel.Result;
using Identity.Application.Contracts;
using Identity.Application.Security;
using MassTransit;

namespace Identity.Application.Commands.ForgotPassword;

public sealed class ForgotPasswordCommandHandler(
    IUserRepository repository,
    IPublishEndpoint publishEndpoint)
    : ICommandHandler<ForgotPasswordCommand, Result>
{
    public async Task<Result> Handle(ForgotPasswordCommand request, CancellationToken ct)
    {
        var user = await repository.GetByEmailAsync(request.Email, ct);
        if (user is null)
            return Result.Success();

        var token = AccountTokenGenerator.Generate();
        var expiresAt = DateTime.Now.AddHours(1);
        user.RequestPasswordReset(AccountTokenGenerator.Hash(token), expiresAt);

        await publishEndpoint.Publish(
            new UserPasswordResetRequestedIntegrationEvent(
                user.Id.Value,
                user.Id.Value,
                user.Name,
                user.Email.Value,
                token,
                expiresAt),
            context => context.CorrelationId = user.Id.Value,
            ct);
        await repository.SaveChangesAsync(ct);
        return Result.Success();
    }
}
