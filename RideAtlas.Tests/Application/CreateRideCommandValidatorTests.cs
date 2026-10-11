using RideAtlas.Application.Rides.Commands;
using Xunit;

namespace RideAtlas.Tests.Application;

public class CreateRideCommandValidatorTests
{
    private readonly CreateRideCommandValidator _validator = new();

    private static CreateRideCommand ValidCommand() => new(
        RiderAccountId: Guid.NewGuid(),
        BikeId: Guid.NewGuid(),
        TripId: null,
        StartedAt: new DateTime(2026, 8, 1, 8, 0, 0, DateTimeKind.Utc),
        EndedAt: new DateTime(2026, 8, 1, 12, 0, 0, DateTimeKind.Utc),
        DistanceKm: 150,
        AvgSpeedKmh: 60,
        MaxSpeedKmh: 100,
        RouteDescription: "Pune to Lonavala",
        WeatherCondition: "Clear",
        WeatherTempC: 28,
        Notes: "Great ride");

    [Fact]
    public void Valid_command_passes()
    {
        var result = _validator.Validate(ValidCommand());
        Assert.True(result.IsValid);
    }

    [Fact]
    public void Missing_bike_id_fails()
    {
        var command = ValidCommand() with { BikeId = Guid.Empty };
        var result = _validator.Validate(command);
        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.PropertyName == nameof(CreateRideCommand.BikeId));
    }

    [Fact]
    public void Zero_distance_fails()
    {
        var command = ValidCommand() with { DistanceKm = 0 };
        var result = _validator.Validate(command);
        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.PropertyName == nameof(CreateRideCommand.DistanceKm));
    }

    [Fact]
    public void End_before_start_fails()
    {
        var command = ValidCommand() with { EndedAt = ValidCommand().StartedAt.AddHours(-1) };
        var result = _validator.Validate(command);
        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.PropertyName == nameof(CreateRideCommand.EndedAt));
    }
}
