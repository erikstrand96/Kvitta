using Infrastructure.Database.Context;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Infrastructure.Database.Extensions;

public static class KvittaDbContextExtensions
{
    public static IServiceCollection ApplyMigrations(this IServiceCollection services)
    {
        using (var scope = services.BuildServiceProvider().CreateScope())
        {
            using (var context = scope.ServiceProvider.GetRequiredService<KvittaDbContext>())
            {
                if (context.Database.GetMigrations().Any())
                {
                    context.Database.Migrate();
                    Console.WriteLine("{0} migration(s) applied", context.Database.GetMigrations().Count());
                }

                return services;
            }
        }
    }

    public static IServiceCollection AddKvittaDbContext(this IServiceCollection services, string connectionString)
    {
        services.AddDbContext<KvittaDbContext>(options =>
            options.UseNpgsql(connectionString));

        return services;
    }
}