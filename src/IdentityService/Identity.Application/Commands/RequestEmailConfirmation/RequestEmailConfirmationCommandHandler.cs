using BuildingBlocks.Contracts.Users;
using BuildingBlocks.SharedKernel.CQRS;
using BuildingBlocks.SharedKernel.Result;
using Identity.Application.Contracts;
using Identity.Application.Security;
using MassTransit;

namespace Identity.Application.Commands.RequestEmailConfirmation;

public sealed class RequestEmailConfirmationCommandHandler(
    IUserRepository repository,
    IPublishEndpoint publishEndpoint)
    : ICommandHandler<RequestEmailConfirmationCommand, Result>
{
    public async Task<Result> Handle(
        RequestEmailConfirmationCommand request,
        CancellationToken ct)
    {
        var user = await repository.GetByEmailAsync(request.Email, ct);
        if (user is null || user.EmailConfirmed)
            return Result.Success();

        var token = AccountTokenGenerator.Generate();
        var expiresAt = DateTime.Now.AddHours(24);
        user.RequestEmailConfirmation(AccountTokenGenerator.Hash(token), expiresAt);
        await publishEndpoint.Publish(
            new UserEmailConfirmationRequestedIntegrationEvent(
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
