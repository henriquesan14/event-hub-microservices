using BuildingBlocks.SharedKernel.CQRS;
using BuildingBlocks.SharedKernel.Result;
using Identity.Application.Dtos;

namespace Identity.Application.Commands.UpdateProfile;

public sealed record UpdateProfileCommand(string Name, string Email) : ICommand<ResultT<UserResponse>>;
