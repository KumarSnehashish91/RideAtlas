using MediatR;
using RideAtlas.Application.AiPlanning.Dtos;
using RideAtlas.Application.Common.Interfaces;

namespace RideAtlas.Application.AiPlanning.Queries;

public record RecommendTripsQuery(RidePlanningInputsDto Inputs) : IRequest<IReadOnlyList<TripRecommendationDto>>;

public class RecommendTripsQueryHandler(IAiPlanningService aiPlanningService)
    : IRequestHandler<RecommendTripsQuery, IReadOnlyList<TripRecommendationDto>>
{
    public Task<IReadOnlyList<TripRecommendationDto>> Handle(RecommendTripsQuery request, CancellationToken ct) =>
        aiPlanningService.RecommendTripsAsync(request.Inputs, ct);
}
