using FluentAssertions;
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
            PurchaseDate = DateTimeOffset.UtcNow,
            Description = "Test Description"
        };

        //Act

        await DbContext.Valuables.AddAsync(valuable);
        await DbContext.SaveChangesAsync();

        //Assert
        var result = await DbContext.Valuables.Include(x => x.Warranty).FirstOrDefaultAsync(x => x.Id == valuable.Id);

        result.Should().NotBeNull();
        result!.Warranty.Should().BeNull("No warranty was added at creation");

    }

    [Fact]
    public async void CreateWithWarranty_ShouldCreateValuableAndWarranty()
    {
        Valuable valuableWithWarranty = new()
        {
            Name = "Test",
            Value = 100,
            PurchaseDate = DateTimeOffset.UtcNow,
            Description = "test description",
            Warranty = new Warranty()
            {
                Description = "warranty description test",
                ExpirationDate = new DateOnly(2025, 01, 01)
            }
        };
        
        //Act

        await DbContext.Valuables.AddAsync(valuableWithWarranty);
        await DbContext.SaveChangesAsync();

        //Assert
        var result = await DbContext.Valuables.Include(x => x.Warranty).FirstOrDefaultAsync(x => x.Id == valuableWithWarranty.Id);

        result.Should().NotBeNull();
        result!.Warranty.Should().NotBeNull("Warranty was added at creation");
        
    }
}