using Microsoft.EntityFrameworkCore;
using Npgsql;

namespace CoreGoDelivery.Infrastructure.Database;

internal static class PostgreSqlConstraintErrors
{
    public static bool IsUniqueViolation(DbUpdateException exception)
    {
        for (var inner = exception.InnerException; inner is not null; inner = inner.InnerException)
        {
            if (inner is PostgresException { SqlState: PostgresErrorCodes.UniqueViolation })
            {
                return true;
            }
        }

        return false;
    }
}
