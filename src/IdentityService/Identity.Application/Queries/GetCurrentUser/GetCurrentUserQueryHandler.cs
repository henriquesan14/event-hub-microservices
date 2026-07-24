using BuildingBlocks.SharedKernel.Abstractions;
using BuildingBlocks.SharedKernel.CQRS;
using BuildingBlocks.SharedKernel.Result;
using Identity.Application.Contracts;
using Identity.Application.Dtos;
using Identity.Application.Errors;
using Identity.Application.Extensions;

namespace Identity.Application.Queries.GetCurrentUser;

public sealed class GetCurrentUserQueryHandler(IUserRepository userRepository, IUserContext userContext)
    : IQueryHandler<GetCurrentUserQuery, ResultT<UserResponse>>
{
    public async Task<ResultT<UserResponse>> Handle(GetCurrentUserQuery request, CancellationToken ct)
    {
        if (userContext.UserId is not Guid userId)
            return AuthErrors.SessionExpired();

        var user = await userRepository.GetByIdAsync(userId, ct);
        return user is null ? UserErrors.NotFound(userId) : user.ToDto();
    }
}
