using BuildingBlocks.SharedKernel.CQRS;
using BuildingBlocks.SharedKernel.Result;
using Identity.Application.Dtos;

namespace Identity.Application.Commands.GenerateAccessToken;

public sealed record GenerateAccessTokenCommand(string Email, string Password) : ICommand<ResultT<AuthResponse>>;
