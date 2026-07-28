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
        if (Email != email)
        {
            EmailConfirmed = false;
            EmailConfirmationTokenHash = null;
            EmailConfirmationTokenExpiresAt = null;
        }

        Name = name;
        Email = email;
        Role = role;
    }

    private readonly List<RefreshToken> _refreshTokens = [];
    public string Name { get; private set; } = default!;
    public Email Email { get; private set; } = default!;
    public string PasswordHash { get; private set; } = default!;
    public UserRole Role { get; private set; } = default!;
    public bool EmailConfirmed { get; private set; }
    public string? EmailConfirmationTokenHash { get; private set; }
    public DateTime? EmailConfirmationTokenExpiresAt { get; private set; }
    public string? PasswordResetTokenHash { get; private set; }
    public DateTime? PasswordResetTokenExpiresAt { get; private set; }

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

    public bool ChangePassword(string oldPassword, string newPassword, IPasswordCheck checker, IPasswordHash hasher)
    {
        if (!checker.Matches(oldPassword, PasswordHash))
            return false;

        PasswordHash = hasher.HashPassword(newPassword);
        return true;
    }

    public void RequestEmailConfirmation(string tokenHash, DateTime expiresAt)
    {
        EmailConfirmationTokenHash = tokenHash;
        EmailConfirmationTokenExpiresAt = expiresAt;
    }

    public bool ConfirmEmail(string tokenHash, DateTime now)
    {
        if (EmailConfirmed)
            return true;
        if (EmailConfirmationTokenHash != tokenHash ||
            EmailConfirmationTokenExpiresAt is null ||
            EmailConfirmationTokenExpiresAt < now)
            return false;

        EmailConfirmed = true;
        EmailConfirmationTokenHash = null;
        EmailConfirmationTokenExpiresAt = null;
        return true;
    }

    public void RequestPasswordReset(string tokenHash, DateTime expiresAt)
    {
        PasswordResetTokenHash = tokenHash;
        PasswordResetTokenExpiresAt = expiresAt;
    }

    public bool ResetPassword(
        string tokenHash,
        string newPassword,
        DateTime now,
        IPasswordHash hasher)
    {
        if (PasswordResetTokenHash != tokenHash ||
            PasswordResetTokenExpiresAt is null ||
            PasswordResetTokenExpiresAt < now)
            return false;

        PasswordHash = hasher.HashPassword(newPassword);
        PasswordResetTokenHash = null;
        PasswordResetTokenExpiresAt = null;
        return true;
    }
}
