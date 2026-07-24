using BuildingBlocks.SharedKernel.CQRS;
using BuildingBlocks.SharedKernel.Result;
using Identity.Application.Dtos;

namespace Identity.Application.Queries.GetCurrentUser;

public sealed record GetCurrentUserQuery : IQuery<ResultT<UserResponse>>;
