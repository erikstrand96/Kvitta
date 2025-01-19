using System.Net;
using System.Net.Http.Json;
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
        var response = await HttpClient.PostAsJsonAsync("/valuables", valuable);
        
        //Assert
        Assert.Equivalent(response.StatusCode, HttpStatusCode.Created);
        var locationHeader = response.Headers.Location;
        Assert.NotNull(locationHeader);
    }

    [Fact]
    public async Task GetAllValuables_ShouldReturnCollectionWithValues()
    {
        const string uri = "/valuables";

        var response = await HttpClient.GetFromJsonAsync<List<Valuable>>(uri);

        Assert.True(response!.Count > 0);
    }

    [Fact]
    public async Task DeleteValuable_ShouldDeleteEntity()
    {
        var entity = await DbContext.Valuables.FirstAsync(x => x.Name.Equals("DeleteValuable"));
        string uri = $"/valuables/{entity.Id}";
        
        var response = await HttpClient.DeleteAsync(uri);

        Assert.Equivalent(response.StatusCode, HttpStatusCode.OK);
    }
}