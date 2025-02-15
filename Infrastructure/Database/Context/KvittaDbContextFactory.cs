using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace Infrastructure.Database.Context;

// ReSharper disable once UnusedType.Global
internal class KvittaDbContextFactory : IDesignTimeDbContextFactory<KvittaDbContext>
{
    public KvittaDbContext CreateDbContext(string[] args)
    {
        var optionsBuilder = new DbContextOptionsBuilder<KvittaDbContext>();

        string connectionString = Environment.GetEnvironmentVariable("KvittaDbConnection") ??
                                  throw new InvalidOperationException("KvittaDbConnection is not set.");

        optionsBuilder.UseNpgsql(connectionString);

        return new KvittaDbContext(optionsBuilder.Options);
    }
}