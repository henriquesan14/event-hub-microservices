using BuildingBlocks.SharedKernel.CQRS;
using BuildingBlocks.SharedKernel.Result;
using Identity.Application.Contracts;
using Identity.Application.Dtos;
using Identity.Application.Errors;
using Identity.Application.Extensions;
using Identity.Domain.Contracts;
using Identity.Domain.Entities;
using Identity.Domain.ValueObjects;
using BuildingBlocks.Contracts.Users;
using MassTransit;

namespace Identity.Application.Commands.RegisterUser;

public sealed class RegisterUserCommandHandler(
    IUserRepository userRepository,
    IPasswordHash passwordHash,
    IPublishEndpoint publishEndpoint)
    : ICommandHandler<RegisterUserCommand, ResultT<UserResponse>>
{
    public async Task<ResultT<UserResponse>> Handle(RegisterUserCommand request, CancellationToken ct)
    {
        var emailExist = await userRepository.EmailExistsAsync(request.Email, ct);
        if (emailExist) return UserErrors.Conflict(request.Email);

        var user = User.CreateUser(UserId.New(), request.Name, Email.Of(request.Email), request.Password, passwordHash);
        await userRepository.AddAsync(user, ct);
        await publishEndpoint.Publish(
            new UserRegisteredIntegrationEvent(
                user.Id.Value,
                user.Id.Value,
                user.Name,
                user.Email.Value),
            context => context.CorrelationId = user.Id.Value,
            ct);
        await userRepository.SaveChangesAsync(ct);

        return user.ToDto();
    }
}
