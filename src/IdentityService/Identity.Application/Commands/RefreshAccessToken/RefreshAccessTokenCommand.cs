using BuildingBlocks.SharedKernel.CQRS;
using BuildingBlocks.SharedKernel.Result;
using Identity.Application.Dtos;

namespace Identity.Application.Commands.RefreshAccessToken;

public sealed record RefreshAccessTokenCommand : ICommand<ResultT<AuthResponse>>;
