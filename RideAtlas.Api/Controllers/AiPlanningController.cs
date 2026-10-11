using MediatR;
using Microsoft.AspNetCore.Mvc;
using RideAtlas.Application.AiPlanning.Dtos;
using RideAtlas.Application.AiPlanning.Queries;

namespace RideAtlas.Api.Controllers;

public record PlanMyRideRequest(string CompanionType, IReadOnlyList<string> Interests, string TimeAvailable, string RidingStyle);

public record TripBriefingRequest(string Destination, string Origin, double DistanceKm, int RideDurationMinutes, string? Motorcycle, string RidingStyle);

public record JournalDraftRequest(string RiderPrompt);

public class AiPlanningController(ISender sender) : ApiControllerBase(sender)
{
    [HttpPost("plan-my-ride")]
    public async Task<ActionResult<IReadOnlyList<TripRecommendationDto>>> PlanMyRide(PlanMyRideRequest request, CancellationToken ct)
    {
        var inputs = new RidePlanningInputsDto(request.CompanionType, request.Interests, request.TimeAvailable, request.RidingStyle);
        var recommendations = await Sender.Send(new RecommendTripsQuery(inputs), ct);
        return Ok(recommendations);
    }

    [HttpPost("trip-briefing")]
    public async Task<ActionResult<TripBriefingDto>> TripBriefing(TripBriefingRequest request, CancellationToken ct)
    {
        var context = new TripBriefingContext(
            request.Destination, request.Origin, request.DistanceKm, request.RideDurationMinutes,
            request.Motorcycle, request.RidingStyle);
        var briefing = await Sender.Send(new GenerateTripBriefingQuery(context), ct);
        return Ok(briefing);
    }

    [HttpPost("ride-story/{rideId:guid}")]
    public async Task<ActionResult<RideStoryDto>> RideStory(Guid rideId, CancellationToken ct)
    {
        var story = await Sender.Send(new GenerateRideStoryQuery(rideId, RiderAccountId), ct);
        return Ok(story);
    }

    [HttpPost("journal-draft")]
    public async Task<ActionResult<JournalDraftDto>> JournalDraft(JournalDraftRequest request, CancellationToken ct)
    {
        var draft = await Sender.Send(new GenerateJournalDraftQuery(request.RiderPrompt), ct);
        return Ok(draft);
    }
}
