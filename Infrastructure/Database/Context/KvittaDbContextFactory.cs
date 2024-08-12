using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;

namespace Infrastructure.Database.Context;

internal class KvittaDbContextFactory : IDesignTimeDbContextFactory<KvittaDbContext>
{
    
    public KvittaDbContext CreateDbContext(string[] args)
    {
        var optionsBuilder = new DbContextOptionsBuilder<KvittaDbContext>();
        
        string connectionString = Environment.GetEnvironmentVariable("POSTGRES_CONNECTION") ?? throw new InvalidOperationException("POSTGRES_CONNECTION is not set.");
        
        optionsBuilder.UseNpgsql(connectionString);
        
        return new KvittaDbContext(optionsBuilder.Options);
    }
}