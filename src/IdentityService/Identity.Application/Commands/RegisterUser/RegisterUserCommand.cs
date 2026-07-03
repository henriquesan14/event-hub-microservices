using BuildingBlocks.SharedKernel.CQRS;
using BuildingBlocks.SharedKernel.Result;
using Identity.Application.Dtos;

namespace Identity.Application.Commands.RegisterUser;

public sealed record RegisterUserCommand(string Name, string Email, string Password) : ICommand<ResultT<UserResponse>>;
