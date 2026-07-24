using BuildingBlocks.SharedKernel.Abstractions;
using BuildingBlocks.SharedKernel.CQRS;
using BuildingBlocks.SharedKernel.Result;
using Identity.Application.Contracts;
using Identity.Application.Dtos;
using Identity.Application.Errors;
using Identity.Application.Extensions;
using Identity.Domain.ValueObjects;

namespace Identity.Application.Commands.UpdateProfile;

public sealed class UpdateProfileCommandHandler(IUserRepository userRepository, IUserContext userContext)
    : ICommandHandler<UpdateProfileCommand, ResultT<UserResponse>>
{
    public async Task<ResultT<UserResponse>> Handle(UpdateProfileCommand request, CancellationToken ct)
    {
        if (userContext.UserId is not Guid userId)
            return AuthErrors.SessionExpired();

        var user = await userRepository.GetByIdAsync(userId, ct);
        if (user is null)
            return UserErrors.NotFound(userId);

        if (!string.Equals(user.Email.Value, request.Email, StringComparison.OrdinalIgnoreCase) &&
            await userRepository.EmailExistsAsync(request.Email, ct))
            return UserErrors.Conflict(request.Email);

        user.Update(request.Name, Email.Of(request.Email), user.Role);
        await userRepository.SaveChangesAsync(ct);
        return user.ToDto();
    }
}
