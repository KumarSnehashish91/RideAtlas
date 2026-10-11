using MediatR;
using RideAtlas.Application.Common.Exceptions;
using RideAtlas.Application.Common.Interfaces;
using RideAtlas.Application.Rides.Dtos;
using RideAtlas.Domain.Entities;

namespace RideAtlas.Application.Rides.Commands;

public record LinkRideToTripCommand(Guid RideId, Guid TripId, Guid RiderAccountId) : IRequest<RideDto>;

public class LinkRideToTripCommandHandler(
    IRideRepository rideRepository,
    ITripRepository tripRepository) : IRequestHandler<LinkRideToTripCommand, RideDto>
{
    public async Task<RideDto> Handle(LinkRideToTripCommand request, CancellationToken ct)
    {
        var ride = await rideRepository.GetByIdAsync(request.RideId, request.RiderAccountId, ct)
            ?? throw new NotFoundException(nameof(Ride), request.RideId);
        var trip = await tripRepository.GetByIdAsync(request.TripId, request.RiderAccountId, ct)
            ?? throw new NotFoundException(nameof(Trip), request.TripId);

        ride.TripId = trip.Id;
        await rideRepository.UpdateAsync(ride, ct);

        trip.CompleteRide();
        await tripRepository.UpdateAsync(trip, ct);

        return ride.ToDto();
    }
}
