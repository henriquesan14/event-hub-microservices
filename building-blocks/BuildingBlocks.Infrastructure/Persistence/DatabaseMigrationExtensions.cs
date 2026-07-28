using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace BuildingBlocks.Infrastructure.Persistence;

public static class DatabaseMigrationExtensions
{
    public static async Task MigrateDatabaseAsync<TContext>(
        this IServiceProvider services,
        bool migrateOnStartup,
        CancellationToken cancellationToken = default)
        where TContext : DbContext
    {
        if (!migrateOnStartup)
            return;

        await using var scope = services.CreateAsyncScope();
        var context = scope.ServiceProvider.GetRequiredService<TContext>();
        await context.Database.MigrateAsync(cancellationToken);
    }
}
