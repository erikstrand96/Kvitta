using Infrastructure.Database.Context;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Testcontainers.PostgreSql;

namespace Kvitta.Integration.Tests;

public class IntegrationTestFactory : WebApplicationFactory<Program>, IAsyncLifetime
{
    private readonly PostgreSqlContainer _dbContainer = new PostgreSqlBuilder()
        .WithDatabase("kvitta")
        .WithUsername("kvitta-user")
        .WithPassword("kvitta-pw")
        .Build();

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        Environment.SetEnvironmentVariable("ASPNETCORE_ENVIRONMENT", "Development");
        
        builder.ConfigureTestServices(services =>
        {
            Type contextType = typeof(DbContextOptions<KvittaDbContext>);

            var context = services.SingleOrDefault(x => x.ServiceType == contextType);

            if (context is not null)
            {
                services.Remove(context);
            }

            services.AddDbContext<KvittaDbContext>(options =>
            {
                string connectionString = _dbContainer.GetConnectionString();
                options.UseNpgsql(connectionString);
            });
        });
    }

    public Task InitializeAsync()
    {
        return _dbContainer.StartAsync();
    }

    public new Task DisposeAsync()
    {
        return _dbContainer.StopAsync();
    }
}