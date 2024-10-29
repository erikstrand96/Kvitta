using Infrastructure.Database.Context;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Kvitta.Integration.Tests;

public abstract class BaseIntegrationTest : IClassFixture<IntegrationTestFactory>, IDisposable
{
    private readonly IServiceScope _scope;
    protected readonly KvittaDbContext DbContext;

    protected BaseIntegrationTest(IntegrationTestFactory factory)
    {
        _scope = factory.Services.CreateScope();

        DbContext = _scope.ServiceProvider.GetRequiredService<KvittaDbContext>();

        if (DbContext.Database.GetMigrations().Any())
        {
            DbContext.Database.Migrate();
        }
    }
    
    public void Dispose()
    {
        _scope?.Dispose();
        DbContext?.Dispose();
    }
}