using System.Net;
using System.Net.Http.Json;
using FluentAssertions;
using Infrastructure.Database.Context;
using Infrastructure.Database.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

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
        response.StatusCode.Should().Be(HttpStatusCode.Created);

        var locationHeader = response.Headers.Location;
        locationHeader.Should().NotBeNull();
    }

    [Fact]
    public async Task GetAllValuables_ShouldReturnCollectionWithValues()
    {
        const string uri = "/valuables";

        var response = await HttpClient.GetFromJsonAsync<List<Valuable>>(uri);

        response!.Count.Should().BeGreaterThan(0);
    }

    [Fact]
    public async Task DeleteValuable_ShouldDeleteEntity()
    {
        Valuable entity;

        await using (var scope = testFactory.Services.CreateAsyncScope())
        {
            await using (var context = scope.ServiceProvider.GetRequiredService<KvittaDbContext>())
            {
                entity = await context.Valuables.FirstAsync(x => x.Name.Equals("DeleteValuable"));
            }
        }

        string uri = $"/valuables/{entity.Id}";
        var response = await HttpClient.DeleteAsync(uri);

        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }
}