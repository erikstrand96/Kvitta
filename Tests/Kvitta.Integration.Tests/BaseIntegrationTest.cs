using Infrastructure.Database.Context;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Kvitta.Integration.Tests;

public abstract class BaseIntegrationTest : IClassFixture<IntegrationTestFactory>, IDisposable
{
    private readonly IServiceScope _scope;
    private readonly KvittaDbContext _dbContext;
    protected readonly HttpClient HttpClient;

    protected BaseIntegrationTest(IntegrationTestFactory factory)
    {
        _scope = factory.Services.CreateScope();

        _dbContext = _scope.ServiceProvider.GetRequiredService<KvittaDbContext>();

        HttpClient = factory.CreateClient();
        
        if (_dbContext.Database.GetMigrations().Any())
        {
            _dbContext.Database.Migrate();
        }
    }
    
    public void Dispose()
    {
        _scope?.Dispose();
        _dbContext?.Dispose();
        HttpClient?.Dispose();;
    }
}