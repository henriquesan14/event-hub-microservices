using BuildingBlocks.SharedKernel.CQRS;
using BuildingBlocks.SharedKernel.Result;
using Identity.Application.Dtos;
using Identity.Domain.Enums;

namespace Identity.Application.Commands.ChangeUserRole;

public sealed record ChangeUserRoleCommand(
    Guid UserId,
    UserRole Role) : ICommand<ResultT<UserResponse>>;
