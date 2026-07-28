using Identity.Application.Contracts;
using Identity.Domain.Entities;
using Identity.Domain.ValueObjects;
using Microsoft.EntityFrameworkCore;

namespace Identity.Infrastructure.Persistence.Repositories;

public sealed class UserRepository(IdentityDbContext context) : IUserRepository
{
    public async Task<User> AddAsync(User user, CancellationToken ct)
    {
        await context.Set<User>().AddAsync(user, ct);
        return user;
    }

    public async Task<bool> EmailExistsAsync(string email, CancellationToken ct)
    {
        return await context.Set<User>().AnyAsync(u => u.Email == Email.Of(email), ct);
    }

    public async Task<User?> GetByIdAsync(Guid id, CancellationToken ct)
    {
        return await context.Set<User>()
            .FirstOrDefaultAsync(u => u.Id == UserId.Of(id), ct);
    }

    public async Task<User?> GetByEmailAsync(string email, CancellationToken ct)
    {
        return await context.Set<User>().FirstOrDefaultAsync(u => u.Email == Email.Of(email), ct);
    }

    public Task<User?> GetByEmailConfirmationTokenHashAsync(
        string tokenHash,
        CancellationToken ct) =>
        context.Set<User>().FirstOrDefaultAsync(
            user => user.EmailConfirmationTokenHash == tokenHash,
            ct);

    public Task<User?> GetByPasswordResetTokenHashAsync(
        string tokenHash,
        CancellationToken ct) =>
        context.Set<User>().FirstOrDefaultAsync(
            user => user.PasswordResetTokenHash == tokenHash,
            ct);

    public async Task<RefreshToken?> GetByRefreshTokenAsync(string token, CancellationToken ct)
    {
        return await context.Set<RefreshToken>()
            .Include(rt => rt.User)
            .FirstOrDefaultAsync(rt => rt.Token == token, ct);
    }

    public async Task<int> SaveChangesAsync(CancellationToken ct)
    {
        return await context.SaveChangesAsync(ct);
    }
}
