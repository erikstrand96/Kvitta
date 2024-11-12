using Infrastructure.Database.Context;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Testcontainers.PostgreSql;

namespace Kvitta.Integration.Tests;

// ReSharper disable once ClassNeverInstantiated.Global
public class IntegrationTestFactory : WebApplicationFactory<Program>, IAsyncLifetime
{
    private readonly PostgreSqlContainer _dbContainer = new PostgreSqlBuilder()
        .WithDatabase("test")
        .WithUsername("test-user")
        .WithPassword("test-pw")
        .Build();

    /// <summary>
    /// Configure in-memory representation of the web host, used for testing
    /// </summary>
    /// <param name="builder"></param>
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        Environment.SetEnvironmentVariable("ASPNETCORE_ENVIRONMENT", "Development");

        string connectionString = _dbContainer.GetConnectionString();
        Environment.SetEnvironmentVariable("KvittaDbConnection", connectionString);

        builder.ConfigureTestServices(services =>
        {
            RemoveExistingDbContext(services);

            services.AddDbContext<KvittaDbContext>(options =>
            {
                options.UseNpgsql(connectionString);
            });
        });
        
    }

    private static void RemoveExistingDbContext(IServiceCollection services)
    {
        Type contextType = typeof(DbContextOptions<KvittaDbContext>);
        ServiceDescriptor? serviceDescriptor = services.SingleOrDefault(x => x.ServiceType == contextType);
        if (serviceDescriptor is not null)
        {
            services.Remove(serviceDescriptor);
        }
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