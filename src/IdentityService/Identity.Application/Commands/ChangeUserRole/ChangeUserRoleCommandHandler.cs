using BuildingBlocks.SharedKernel.CQRS;
using BuildingBlocks.SharedKernel.Result;
using Identity.Application.Contracts;
using Identity.Application.Dtos;
using Identity.Application.Errors;

namespace Identity.Application.Commands.ChangeUserRole;

public sealed class ChangeUserRoleCommandHandler(IUserRepository userRepository)
    : ICommandHandler<ChangeUserRoleCommand, ResultT<UserResponse>>
{
    public async Task<ResultT<UserResponse>> Handle(
        ChangeUserRoleCommand request,
        CancellationToken ct)
    {
        var user = await userRepository.GetByIdAsync(request.UserId, ct);
        if (user is null)
            return UserErrors.NotFound(request.UserId);

        user.ChangeRole(request.Role);
        await userRepository.SaveChangesAsync(ct);

        return new UserResponse(
            user.Id.Value,
            user.Name,
            user.Email.Value,
            user.Role,
            user.EmailConfirmed,
            user.CreatedAt,
            user.CreatedByName);
    }
}
