using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace Infrastructure.Database.Context;

internal class KvittaDbContextFactory : IDesignTimeDbContextFactory<KvittaDbContext>
{
    public KvittaDbContext CreateDbContext(string[] args)
    {
        var optionsBuilder = new DbContextOptionsBuilder<KvittaDbContext>();

        string connectionString = Environment.GetEnvironmentVariable("KVITTA_DB_CONNECTION") ??
                                  throw new InvalidOperationException("KVITTA_DB_CONNECTION is not set.");

        optionsBuilder.UseNpgsql(connectionString);

        return new KvittaDbContext(optionsBuilder.Options);
    }
}