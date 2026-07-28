using Identity.Domain.Entities;

namespace Identity.Application.Contracts;

public interface IUserRepository
{
    public Task<User> AddAsync(User user, CancellationToken ct);
    public Task<User?> GetByIdAsync(Guid id, CancellationToken ct);
    public Task<User?> GetByEmailAsync(string email, CancellationToken ct);
    public Task<User?> GetByEmailConfirmationTokenHashAsync(string tokenHash, CancellationToken ct);
    public Task<User?> GetByPasswordResetTokenHashAsync(string tokenHash, CancellationToken ct);
    public Task<RefreshToken?> GetByRefreshTokenAsync(string token, CancellationToken ct);

    public Task<bool> EmailExistsAsync(string email, CancellationToken ct);

    public Task<int> SaveChangesAsync(CancellationToken ct);
};
