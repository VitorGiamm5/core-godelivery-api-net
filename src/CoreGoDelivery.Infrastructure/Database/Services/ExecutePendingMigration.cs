using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace CoreGoDelivery.Infrastructure.Database.Services;

public static class ExecutePendingMigration
{
    public static async Task ExecuteAsync(IServiceProvider services, CancellationToken cancellationToken = default)
    {
        await using var scope = services.CreateAsyncScope();

        var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

        var migrations = await dbContext.Database.GetPendingMigrationsAsync(cancellationToken);
        if (migrations.Any())
        {
            await dbContext.Database.MigrateAsync(cancellationToken);
        }
    }
}
