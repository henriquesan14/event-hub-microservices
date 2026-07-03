using Identity.Domain.Entities;

namespace Identity.Application.Contracts;

public interface IUserRepository
{
    public Task<User> AddAsync(User user, CancellationToken ct);
    public Task<User?> GetByEmailAsync(string email, CancellationToken ct);

    public Task<bool> EmailExistsAsync(string email, CancellationToken ct);

    public Task<int> SaveChangesAsync(CancellationToken ct);
};