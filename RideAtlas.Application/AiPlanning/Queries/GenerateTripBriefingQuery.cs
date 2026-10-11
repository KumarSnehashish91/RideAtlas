using MediatR;
using RideAtlas.Application.AiPlanning.Dtos;
using RideAtlas.Application.Common.Interfaces;

namespace RideAtlas.Application.AiPlanning.Queries;

public record GenerateTripBriefingQuery(TripBriefingContext Context) : IRequest<TripBriefingDto>;

public class GenerateTripBriefingQueryHandler(IAiPlanningService aiPlanningService)
    : IRequestHandler<GenerateTripBriefingQuery, TripBriefingDto>
{
    public Task<TripBriefingDto> Handle(GenerateTripBriefingQuery request, CancellationToken ct) =>
        aiPlanningService.GenerateTripBriefingAsync(request.Context, ct);
}
