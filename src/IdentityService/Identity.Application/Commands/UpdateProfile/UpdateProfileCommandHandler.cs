using BuildingBlocks.SharedKernel.Abstractions;
using BuildingBlocks.SharedKernel.CQRS;
using BuildingBlocks.SharedKernel.Result;
using Identity.Application.Contracts;
using Identity.Application.Dtos;
using Identity.Application.Errors;
using Identity.Application.Extensions;
using Identity.Domain.ValueObjects;
using BuildingBlocks.Contracts.Users;
using MassTransit;
using Identity.Application.Security;

namespace Identity.Application.Commands.UpdateProfile;

public sealed class UpdateProfileCommandHandler(
    IUserRepository userRepository,
    IUserContext userContext,
    IPublishEndpoint publishEndpoint)
    : ICommandHandler<UpdateProfileCommand, ResultT<UserResponse>>
{
    public async Task<ResultT<UserResponse>> Handle(UpdateProfileCommand request, CancellationToken ct)
    {
        if (userContext.UserId is not Guid userId)
            return AuthErrors.SessionExpired();

        var user = await userRepository.GetByIdAsync(userId, ct);
        if (user is null)
            return UserErrors.NotFound(userId);

        var emailChanged =
            !string.Equals(user.Email.Value, request.Email, StringComparison.OrdinalIgnoreCase);
        if (emailChanged &&
            await userRepository.EmailExistsAsync(request.Email, ct))
            return UserErrors.Conflict(request.Email);

        user.Update(request.Name, Email.Of(request.Email), user.Role);
        await publishEndpoint.Publish(
            new UserUpdatedIntegrationEvent(
                user.Id.Value,
                user.Id.Value,
                user.Name,
                user.Email.Value),
            context => context.CorrelationId = user.Id.Value,
            ct);
        if (emailChanged)
        {
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
        }
        await userRepository.SaveChangesAsync(ct);
        return user.ToDto();
    }
}
