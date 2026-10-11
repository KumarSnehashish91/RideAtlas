using RideAtlas.Domain.Entities;
using RideAtlas.Domain.Enums;
using Xunit;

namespace RideAtlas.Tests.Domain;

public class TripStatusTests
{
    private static Trip MakeTrip(DateOnly start, DateOnly end) => new()
    {
        Name = "Test Trip",
        StartDate = start,
        EndDate = end
    };

    private static readonly DateOnly Today = new(2026, 8, 3);

    [Fact]
    public void Cancelled_trip_is_always_cancelled_regardless_of_other_flags()
    {
        var trip = MakeTrip(Today.AddDays(-5), Today.AddDays(5));
        trip.IsCancelled = true;
        trip.HasRideCompleted = true;

        Assert.Equal(TripStatus.Cancelled, trip.Status(Today));
    }

    [Fact]
    public void Draft_trip_is_draft_even_if_dates_overlap_today()
    {
        var trip = MakeTrip(Today.AddDays(-1), Today.AddDays(1));
        trip.IsDraft = true;

        Assert.Equal(TripStatus.Draft, trip.Status(Today));
    }

    [Fact]
    public void Completed_ride_wins_over_dates()
    {
        var trip = MakeTrip(Today.AddDays(10), Today.AddDays(15));
        trip.HasRideCompleted = true;

        Assert.Equal(TripStatus.Completed, trip.Status(Today));
    }

    [Fact]
    public void Trip_spanning_today_is_active()
    {
        var trip = MakeTrip(Today.AddDays(-1), Today.AddDays(1));

        Assert.Equal(TripStatus.Active, trip.Status(Today));
    }

    [Fact]
    public void Explicitly_started_trip_is_active_even_before_start_date()
    {
        var trip = MakeTrip(Today.AddDays(1), Today.AddDays(5));
        trip.HasStarted = true;

        Assert.Equal(TripStatus.Active, trip.Status(Today));
    }

    [Fact]
    public void Future_trip_not_yet_started_is_planned()
    {
        var trip = MakeTrip(Today.AddDays(5), Today.AddDays(10));

        Assert.Equal(TripStatus.Planned, trip.Status(Today));
    }

    [Fact]
    public void CompleteRide_sets_HasRideCompleted()
    {
        var trip = MakeTrip(Today, Today.AddDays(1));

        trip.CompleteRide();

        Assert.True(trip.HasRideCompleted);
        Assert.Equal(TripStatus.Completed, trip.Status(Today));
    }
}
