using System.Net;
using System.Net.Http.Json;
using RideAtlas.Application.Bikes.Dtos;
using RideAtlas.Application.Common.Models;
using RideAtlas.Application.Rides.Dtos;
using RideAtlas.Application.Trips.Dtos;
using RideAtlas.Domain.Enums;
using Xunit;

namespace RideAtlas.Tests.Integration;

public class RidesFlowTests(CustomWebApplicationFactory factory) : IClassFixture<CustomWebApplicationFactory>
{
    private HttpClient NewAuthenticatedClient(out string email)
    {
        var client = factory.CreateClient();
        email = $"rider-{Guid.NewGuid():N}@example.com";
        var register = client.PostAsJsonAsync("/api/v1/auth/register",
            new { fullName = "Marcus Vance", email, password = "correct-horse-battery-staple" }).Result;
        Assert.Equal(HttpStatusCode.OK, register.StatusCode);
        return client;
    }

    [Fact]
    public async Task Logging_a_ride_linked_to_a_trip_completes_the_trip()
    {
        var client = NewAuthenticatedClient(out _);

        var bike = await client.PostAsJsonAsync("/api/v1/bikes", new
        {
            nickname = "Thumper",
            brand = "Royal Enfield",
            model = "Himalayan",
            year = 2023,
            engineCc = 411,
            registrationNumber = "MH12AB1234",
            currentOdometerKm = 5000.0,
            isActive = true
        });
        var bikeDto = await bike.Content.ReadFromJsonAsync<BikeDto>(JsonDefaults.Options);
        Assert.NotNull(bikeDto);

        var trip = await client.PostAsJsonAsync("/api/v1/trips", new
        {
            name = "Ladakh Loop",
            startDate = "2026-08-01",
            endDate = "2026-08-10",
            bikeId = bikeDto!.Id,
            distanceKm = (double?)null,
            budget = (decimal?)null,
            isDraft = false
        });
        var tripDto = await trip.Content.ReadFromJsonAsync<TripDto>(JsonDefaults.Options);
        Assert.NotNull(tripDto);
        Assert.False(tripDto!.HasRideCompleted);

        var ride = await client.PostAsJsonAsync("/api/v1/rides", new
        {
            bikeId = bikeDto.Id,
            tripId = tripDto.Id,
            startedAt = "2026-08-01T06:00:00Z",
            endedAt = "2026-08-01T14:00:00Z",
            distanceKm = 220.5,
            avgSpeedKmh = 45.0,
            maxSpeedKmh = 80.0,
            routeDescription = "Manali to Keylong",
            weatherCondition = "Clear",
            weatherTempC = 12.0,
            notes = "First day of the loop"
        });
        Assert.Equal(HttpStatusCode.OK, ride.StatusCode);
        var rideDto = await ride.Content.ReadFromJsonAsync<RideDto>(JsonDefaults.Options);
        Assert.NotNull(rideDto);
        Assert.Equal(tripDto.Id, rideDto!.TripId);

        var tripAfter = await client.GetFromJsonAsync<TripDto>($"/api/v1/trips/{tripDto.Id}", JsonDefaults.Options);
        Assert.NotNull(tripAfter);
        Assert.True(tripAfter!.HasRideCompleted);
        Assert.Equal(TripStatus.Completed, tripAfter.Status);

        var list = await client.GetFromJsonAsync<PagedResult<RideDto>>(
            $"/api/v1/rides?bikeId={bikeDto.Id}", JsonDefaults.Options);
        Assert.NotNull(list);
        Assert.Single(list!.Items);

        var delete = await client.DeleteAsync($"/api/v1/rides/{rideDto.Id}");
        Assert.Equal(HttpStatusCode.NoContent, delete.StatusCode);

        var getDeleted = await client.GetAsync($"/api/v1/rides/{rideDto.Id}");
        Assert.Equal(HttpStatusCode.NotFound, getDeleted.StatusCode);
    }
}
