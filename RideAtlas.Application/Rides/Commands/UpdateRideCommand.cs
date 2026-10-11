using FluentValidation;
using MediatR;
using RideAtlas.Application.Common.Exceptions;
using RideAtlas.Application.Common.Interfaces;
using RideAtlas.Application.Rides.Dtos;
using RideAtlas.Domain.Entities;

namespace RideAtlas.Application.Rides.Commands;

public record UpdateRideCommand(
    Guid Id,
    Guid RiderAccountId,
    DateTime StartedAt,
    DateTime EndedAt,
    double DistanceKm,
    double? AvgSpeedKmh,
    double? MaxSpeedKmh,
    string? RouteDescription,
    string? WeatherCondition,
    double? WeatherTempC,
    string? Notes) : IRequest<RideDto>;

public class UpdateRideCommandValidator : AbstractValidator<UpdateRideCommand>
{
    public UpdateRideCommandValidator()
    {
        RuleFor(x => x.DistanceKm).GreaterThan(0);
        RuleFor(x => x.EndedAt).GreaterThan(x => x.StartedAt);
        RuleFor(x => x.AvgSpeedKmh).GreaterThan(0).When(x => x.AvgSpeedKmh.HasValue);
        RuleFor(x => x.MaxSpeedKmh).GreaterThan(0).When(x => x.MaxSpeedKmh.HasValue);
    }
}

public class UpdateRideCommandHandler(IRideRepository repository) : IRequestHandler<UpdateRideCommand, RideDto>
{
    public async Task<RideDto> Handle(UpdateRideCommand request, CancellationToken ct)
    {
        var ride = await repository.GetByIdAsync(request.Id, request.RiderAccountId, ct)
            ?? throw new NotFoundException(nameof(Ride), request.Id);

        ride.StartedAt = request.StartedAt;
        ride.EndedAt = request.EndedAt;
        ride.DistanceKm = request.DistanceKm;
        ride.AvgSpeedKmh = request.AvgSpeedKmh;
        ride.MaxSpeedKmh = request.MaxSpeedKmh;
        ride.RouteDescription = request.RouteDescription;
        ride.WeatherCondition = request.WeatherCondition;
        ride.WeatherTempC = request.WeatherTempC;
        ride.Notes = request.Notes;

        await repository.UpdateAsync(ride, ct);
        return ride.ToDto();
    }
}
