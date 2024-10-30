using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using System.Text.Json.Serialization;
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
        var response = await HttpClient.PostAsJsonAsync("/valuables", valuable);
        
        //Assert
        response.StatusCode.Should().Be(HttpStatusCode.Created);
        response.Headers.Location.Should().NotBeNull();
    }
}