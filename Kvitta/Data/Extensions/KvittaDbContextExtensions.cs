using Kvitta.Data.Context;
using Microsoft.EntityFrameworkCore;

namespace Kvitta.Data.Extensions;

public static class KvittaDbContextExtensions
{
    public static IServiceCollection ApplyMigrations(this IServiceCollection services)
    {
        using var scope = services.BuildServiceProvider().CreateScope();
        using var context = scope.ServiceProvider.GetRequiredService<KvittaDbContext>();
        
        if (context.Database.GetMigrations().Any())
        {
            context.Database.Migrate();
        }

        return services;
    }

    public static IServiceCollection AddKvittaDbContext(this IServiceCollection services, string connectionString)
    {
        services.AddDbContext<KvittaDbContext>(options =>
            options.UseNpgsql(connectionString));
        
        return services;
    }
}