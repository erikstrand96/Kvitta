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

        const string query = """

                             INSERT INTO public."Valuables" ("Id", "Name", "PurchaseDate", "Value", "Description")
                             VALUES 
                                 (gen_random_uuid(), 'Gold Ring', '2022-06-15 13:30:00+00', 1500.00, 'A valuable gold ring passed down generations.'),
                                 (gen_random_uuid(), 'Laptop', '2023-02-10 09:00:00+00', 1200.50, 'Latest model, essential for work and personal use.'),
                                 (gen_random_uuid(), 'Guitar', '2021-08-05 18:45:00+00', 800.00, 'Electric guitar used for music production.'),
                                 (gen_random_uuid(), 'Smartphone', '2023-04-20 15:15:00+00', 999.99, 'High-end smartphone with excellent camera features.'),
                                 (gen_random_uuid(), 'Delete', '2020-12-01 14:00:00+00', 300.00, 'Entity to delete.');

                             """;
         
        _dbContext.Database.ExecuteSqlRaw(query);

        _dbContext.SaveChanges();
    }

    public void Dispose()
    {
        _scope?.Dispose();
        _dbContext?.Dispose();
        HttpClient?.Dispose();
    }
}