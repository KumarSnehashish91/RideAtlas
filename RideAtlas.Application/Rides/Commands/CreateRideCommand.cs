using FluentValidation;
using MediatR;
using RideAtlas.Application.Common.Exceptions;
using RideAtlas.Application.Common.Interfaces;
using RideAtlas.Application.Rides.Dtos;
using RideAtlas.Domain.Entities;

namespace RideAtlas.Application.Rides.Commands;

public record CreateRideCommand(
    Guid RiderAccountId,
    Guid BikeId,
    Guid? TripId,
    DateTime StartedAt,
    DateTime EndedAt,
    double DistanceKm,
    double? AvgSpeedKmh,
    double? MaxSpeedKmh,
    string? RouteDescription,
    string? WeatherCondition,
    double? WeatherTempC,
    string? Notes) : IRequest<RideDto>;

public class CreateRideCommandValidator : AbstractValidator<CreateRideCommand>
{
    public CreateRideCommandValidator()
    {
        RuleFor(x => x.BikeId).NotEmpty();
        RuleFor(x => x.DistanceKm).GreaterThan(0);
        RuleFor(x => x.EndedAt).GreaterThan(x => x.StartedAt);
        RuleFor(x => x.AvgSpeedKmh).GreaterThan(0).When(x => x.AvgSpeedKmh.HasValue);
        RuleFor(x => x.MaxSpeedKmh).GreaterThan(0).When(x => x.MaxSpeedKmh.HasValue);
    }
}

public class CreateRideCommandHandler(
    IRideRepository rideRepository,
    IBikeRepository bikeRepository,
    ITripRepository tripRepository) : IRequestHandler<CreateRideCommand, RideDto>
{
    public async Task<RideDto> Handle(CreateRideCommand request, CancellationToken ct)
    {
        _ = await bikeRepository.GetByIdAsync(request.BikeId, request.RiderAccountId, ct)
            ?? throw new NotFoundException(nameof(Motorcycle), request.BikeId);

        Trip? trip = null;
        if (request.TripId is Guid tripId)
        {
            trip = await tripRepository.GetByIdAsync(tripId, request.RiderAccountId, ct)
                ?? throw new NotFoundException(nameof(Trip), tripId);
        }

        var ride = new Ride
        {
            RiderAccountId = request.RiderAccountId,
            BikeId = request.BikeId,
            TripId = request.TripId,
            StartedAt = request.StartedAt,
            EndedAt = request.EndedAt,
            DistanceKm = request.DistanceKm,
            AvgSpeedKmh = request.AvgSpeedKmh,
            MaxSpeedKmh = request.MaxSpeedKmh,
            RouteDescription = request.RouteDescription,
            WeatherCondition = request.WeatherCondition,
            WeatherTempC = request.WeatherTempC,
            Notes = request.Notes
        };
        await rideRepository.AddAsync(ride, ct);

        if (trip is not null)
        {
            trip.CompleteRide();
            await tripRepository.UpdateAsync(trip, ct);
        }

        return ride.ToDto();
    }
}
