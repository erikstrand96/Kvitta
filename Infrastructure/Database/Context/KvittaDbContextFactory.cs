using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;

namespace Infrastructure.Database.Context;

internal class KvittaDbContextFactory : IDesignTimeDbContextFactory<KvittaDbContext>
{
    
    public KvittaDbContext CreateDbContext(string[] args)
    {
        string environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT")!;
        string appsettingsFile = environment.Equals("Development") ? "appsettings.Development.json" : "appsettings.json";
        
        IConfiguration config = new ConfigurationBuilder().AddJsonFile(appsettingsFile).Build();
        string connectionString = config.GetConnectionString("POSTGRES_CONNECTION") ??
                              throw new InvalidOperationException("POSTGRES_CONNECTION is not set.");
        
        var optionsBuilder = new DbContextOptionsBuilder<KvittaDbContext>();
        
        optionsBuilder.UseNpgsql(connectionString);
        
        return new KvittaDbContext(optionsBuilder.Options);
    }
}