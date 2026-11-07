using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Npgsql;

namespace CoreGoDelivery.Infrastructure.Database;

public static class SetupDatabase
{
    public static DbContextOptionsBuilder AddInfrastructure(this DbContextOptionsBuilder optionsBuilder, IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("Postgres");
        if (string.IsNullOrWhiteSpace(connectionString))
        {
            throw new InvalidOperationException("ConnectionStrings:Postgres must be configured.");
        }

        var connectionTimeoutSeconds = configuration.GetValue("Database:ConnectionTimeoutSeconds", 30);
        var includeErrorDetail = configuration.GetValue("Database:IncludeErrorDetail", false);
        var gssEncryptionMode = configuration.GetValue("Database:GssEncryptionMode", GssEncryptionMode.Disable);
        if (connectionTimeoutSeconds is < 1 or > 300)
        {
            throw new InvalidOperationException("Database:ConnectionTimeoutSeconds must be between 1 and 300.");
        }

        var dataSourceBuilder = new NpgsqlDataSourceBuilder(connectionString)
        {
            ConnectionStringBuilder =
            {
                IncludeErrorDetail = includeErrorDetail,
                Timeout = connectionTimeoutSeconds,
                GssEncryptionMode = gssEncryptionMode
            }
        };

        dataSourceBuilder.EnableDynamicJson();

        optionsBuilder
            .UseQueryTrackingBehavior(QueryTrackingBehavior.NoTracking)
            .UseNpgsql(dataSourceBuilder.Build(), b => b
                .MigrationsHistoryTable("__EFMigrationsHistory", "dbgodelivery")
                .MigrationsAssembly(typeof(ApplicationDbContext).Assembly.FullName)
            );

        return optionsBuilder;
    }
}
