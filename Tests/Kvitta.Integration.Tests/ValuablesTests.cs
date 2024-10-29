using Infrastructure.Database.Models;
using Microsoft.EntityFrameworkCore;

namespace Kvitta.Integration.Tests;

public class ValuablesTests(IntegrationTestFactory testFactory) : BaseIntegrationTest(testFactory)
{
    [Fact]
    public async Task Create_ShouldCreateValuable()
    {
        //Arrange
        Valuable valuable = new()
        {
            Name = "Test",
            Value = 100,
            PurchaseDate = DateTimeOffset.UtcNow
        };

        //Act

        await DbContext.Valuables.AddAsync(valuable);
        await DbContext.SaveChangesAsync();

        //Assert
        var result = await DbContext.Valuables.FirstOrDefaultAsync(x => x.Id == valuable.Id);
        Assert.NotNull(result);

    }
}