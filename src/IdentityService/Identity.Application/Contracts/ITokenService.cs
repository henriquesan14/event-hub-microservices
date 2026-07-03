using Identity.Application.Dtos;
using Identity.Domain.Entities;

namespace Identity.Application.Contracts;

public interface ITokenService
{
    TokenResponse GenerateAccessToken(User user);
}
