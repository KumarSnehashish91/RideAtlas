using System.Net;
using System.Net.Http.Json;
using RideAtlas.Application.Bikes.Dtos;
using Xunit;

namespace RideAtlas.Tests.Integration;

public class IsolationTests(CustomWebApplicationFactory factory) : IClassFixture<CustomWebApplicationFactory>
{
    private async Task<HttpClient> RegisterAsync()
    {
        var client = factory.CreateClient();
        var email = $"rider-{Guid.NewGuid():N}@example.com";
        var response = await client.PostAsJsonAsync("/api/v1/auth/register",
            new { fullName = "Test Rider", email, password = "correct-horse-battery-staple" });
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        return client;
    }

    [Fact]
    public async Task Rider_cannot_see_or_delete_another_riders_bike()
    {
        var riderA = await RegisterAsync();
        var riderB = await RegisterAsync();

        var created = await riderA.PostAsJsonAsync("/api/v1/bikes", new
        {
            nickname = "Rider A's Bike",
            brand = "Kawasaki",
            model = "Versys",
            year = 2022,
            engineCc = 650,
            registrationNumber = (string?)null,
            currentOdometerKm = 1000.0,
            isActive = true
        });
        var bike = await created.Content.ReadFromJsonAsync<BikeDto>(JsonDefaults.Options);
        Assert.NotNull(bike);

        var listForB = await riderB.GetFromJsonAsync<List<BikeDto>>("/api/v1/bikes", JsonDefaults.Options);
        Assert.NotNull(listForB);
        Assert.DoesNotContain(listForB!, b => b.Id == bike!.Id);

        var deleteAttempt = await riderB.DeleteAsync($"/api/v1/bikes/{bike!.Id}");
        Assert.Equal(HttpStatusCode.NotFound, deleteAttempt.StatusCode);

        var listForA = await riderA.GetFromJsonAsync<List<BikeDto>>("/api/v1/bikes", JsonDefaults.Options);
        Assert.Contains(listForA!, b => b.Id == bike.Id);
    }
}
