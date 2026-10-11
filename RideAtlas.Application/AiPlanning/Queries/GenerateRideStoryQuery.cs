using MediatR;
using RideAtlas.Application.AiPlanning.Dtos;
using RideAtlas.Application.Common.Exceptions;
using RideAtlas.Application.Common.Interfaces;
using RideAtlas.Domain.Entities;

namespace RideAtlas.Application.AiPlanning.Queries;

public record GenerateRideStoryQuery(Guid RideId, Guid RiderAccountId) : IRequest<RideStoryDto>;

public class GenerateRideStoryQueryHandler(
    IRideRepository rideRepository,
    IAiPlanningService aiPlanningService) : IRequestHandler<GenerateRideStoryQuery, RideStoryDto>
{
    public async Task<RideStoryDto> Handle(GenerateRideStoryQuery request, CancellationToken ct)
    {
        var ride = await rideRepository.GetByIdAsync(request.RideId, request.RiderAccountId, ct)
            ?? throw new NotFoundException(nameof(Ride), request.RideId);

        var context = new RideStoryContext(
            ride.DistanceKm,
            (ride.EndedAt - ride.StartedAt).TotalMinutes,
            ride.AvgSpeedKmh,
            ride.MaxSpeedKmh,
            ride.RouteDescription,
            ride.WeatherCondition,
            ride.Notes);

        return await aiPlanningService.GenerateRideStoryAsync(context, ct);
    }
}
