using Infrastructure.Database.Context;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Kvitta.Integration.Tests;

public abstract class BaseIntegrationTest : IClassFixture<IntegrationTestFactory>, IDisposable
{
    private readonly IServiceScope _scope;
    protected private readonly KvittaDbContext DbContext;
    protected readonly HttpClient HttpClient;

    protected BaseIntegrationTest(IntegrationTestFactory factory)
    {
        _scope = factory.Services.CreateScope();

        DbContext = _scope.ServiceProvider.GetRequiredService<KvittaDbContext>();

        HttpClient = factory.CreateClient();

        if (DbContext.Database.GetMigrations().Any())
        {
            DbContext.Database.Migrate();
        }

        var config = factory.Services.GetRequiredService<IConfiguration>();

        string contentRoot = config.GetValue<string>(WebHostDefaults.ContentRootKey)!;
        DirectoryInfo directoryInfo = new(contentRoot);
        string parentPath = directoryInfo.Parent!.FullName;
        string filePath = "Tests/Kvitta.Integration.Tests/testdata/valuables-data.txt";
        
        string content = File.ReadAllText(Path.Combine(parentPath, filePath));

        DbContext.Database.ExecuteSqlRaw(content);

        DbContext.SaveChanges();
    }

    public void Dispose()
    {
        _scope.Dispose();
        DbContext.Dispose();
        HttpClient.Dispose();
    }
}