using Identity.Domain.Contracts;

namespace Identity.Infrastructure.Services;

public sealed class PasswordService : IPasswordCheck, IPasswordHash
{
    public bool Matches(string password, string hash)
    {
        return BCrypt.Net.BCrypt.Verify(password, hash);
    }

    public string HashPassword(string password)
    {
        return BCrypt.Net.BCrypt.HashPassword(password);
    }
}
