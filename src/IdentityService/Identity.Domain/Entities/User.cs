using BuildingBlocks.SharedKernel.Abstractions;
using Identity.Domain.Contracts;
using Identity.Domain.Enums;
using Identity.Domain.ValueObjects;

namespace Identity.Domain.Entities;

public sealed class User : AggregateRoot<UserId>
{
    private User()
    {
    }

    private User(UserId id, string name, Email email, UserRole role, string password, IPasswordHash passwordHash)
    {
        Id = id;
        Name = name;
        Email = email;
        PasswordHash = passwordHash.HashPassword(password);
        Role = role;
    }

    public static User CreateUser(UserId id, string name, Email email, string password, IPasswordHash passwordHash)
    {
        return new User(id, name, email, UserRole.User, password, passwordHash);
    }

    public void Update(string name, Email email, UserRole role)
    {
        Name = name;
        Email = email;
        Role = role;
    }

    private readonly List<RefreshToken> _refreshTokens = [];
    public string Name { get; private set; } = default!;
    public Email Email { get; private set; } = default!;
    public string PasswordHash { get; private set; } = default!;
    public UserRole Role { get; private set; } = default!;

    public IReadOnlyCollection<RefreshToken> RefreshTokens =>
        _refreshTokens.AsReadOnly();

    public void AddRefreshToken(RefreshToken refreshToken)
    {
        _refreshTokens.Add(refreshToken);
    }

    public bool CanUserLogin(string password, IPasswordCheck checker)
    {
        return checker.Matches(password, PasswordHash);
    }

    public void ChangeRole(UserRole role)
    {
        Role = role;
    }

    public void ChangeName(string name)
    {
        Name = name;
    }

    public void ChangePassword(string oldPassword, string newPassword, IPasswordCheck checker, IPasswordHash hasher)
    {
        PasswordHash = hasher.HashPassword(newPassword);
    }
}
