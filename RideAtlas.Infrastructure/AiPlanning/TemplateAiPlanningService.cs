using RideAtlas.Application.AiPlanning.Dtos;
using RideAtlas.Application.Common.Interfaces;

namespace RideAtlas.Infrastructure.AiPlanning;

/// <summary>Deterministic, template-based stand-in for the self-hosted Ollama/Qwen model
/// (§7.2's IAiPlanningService). Swap this registration for a real client once that infra exists —
/// callers only depend on the interface.</summary>
public class TemplateAiPlanningService : IAiPlanningService
{
    public Task<string> GenerateJournalNarrativeAsync(JournalNarrativeContext context, CancellationToken ct = default)
    {
        var mood = string.IsNullOrWhiteSpace(context.Mood) ? "reflective" : context.Mood!.ToLowerInvariant();
        var distance = context.DistanceKm is double km ? $" over {km:0.#} km" : string.Empty;
        var riderNotes = string.IsNullOrWhiteSpace(context.RiderInputRaw)
            ? string.Empty
            : $" {context.RiderInputRaw!.Trim()}";

        var narrative =
            $"On {context.Date:MMMM d, yyyy}, \"{context.Title}\" unfolded{distance}, leaving a {mood} impression." +
            riderNotes;

        return Task.FromResult(narrative);
    }

    public Task<JournalDraftDto> GenerateJournalDraftAsync(string riderPrompt, CancellationToken ct = default)
    {
        var trimmed = riderPrompt.Trim();
        var title = trimmed.Length > 60 ? string.Concat(trimmed.AsSpan(0, 57), "...") : trimmed;
        var narrative = $"{trimmed} It's a moment worth remembering long after the tyres cool down.";
        return Task.FromResult(new JournalDraftDto(title, "Content", narrative));
    }

    public Task<RideStoryDto> GenerateRideStoryAsync(RideStoryContext context, CancellationToken ct = default)
    {
        var hours = context.DurationMinutes / 60.0;
        var avgSpeedText = context.AvgSpeedKmh is double avg ? $"averaging {avg:0.#} km/h" : "at a steady pace";
        var summary = $"A {context.DistanceKm:0.#} km ride spanning {hours:0.#} hours, {avgSpeedText}.";

        var stats = new List<string> { $"{context.DistanceKm:0.#} km covered", $"{hours:0.#} hours in the saddle" };
        if (context.AvgSpeedKmh is double a) stats.Add($"Average speed {a:0.#} km/h");
        if (context.MaxSpeedKmh is double m) stats.Add($"Peak speed {m:0.#} km/h");

        var highlights = new List<string>();
        if (!string.IsNullOrWhiteSpace(context.RouteDescription)) highlights.Add(context.RouteDescription!);
        if (!string.IsNullOrWhiteSpace(context.WeatherCondition)) highlights.Add($"Weather: {context.WeatherCondition}");

        var narrative = string.IsNullOrWhiteSpace(context.Notes)
            ? summary
            : $"{summary} {context.Notes!.Trim()}";

        return Task.FromResult(new RideStoryDto(
            JourneySummary: summary,
            Highlights: highlights,
            InterestingStatistics: stats,
            MemorableMoments: highlights,
            RidingInsights: [$"Pace held {avgSpeedText}."],
            SafetyObservations: ["No incidents reported for this ride."],
            FutureSuggestions: ["Consider logging fuel and rest stops next time for richer trend data."],
            NarrativeText: narrative));
    }

    public Task<IReadOnlyList<TripRecommendationDto>> RecommendTripsAsync(RidePlanningInputsDto inputs, CancellationToken ct = default)
    {
        var interests = inputs.Interests.Count > 0 ? string.Join(", ", inputs.Interests) : "open roads";
        IReadOnlyList<TripRecommendationDto> recommendations =
        [
            new TripRecommendationDto(
                Destination: "Coorg",
                Region: "Karnataka",
                Summary: $"A {inputs.RidingStyle.ToLowerInvariant()}-friendly loop through misty coffee estates, well suited for {interests}.",
                Difficulty: "Easy",
                RoadType: "Tarmac",
                BestSeason: "Oct-Feb",
                ScenicRating: 4.5,
                FuelEstimate: 1200m,
                BudgetEstimate: 6000m,
                WeatherOverview: "Mild and pleasant",
                RecommendationReason: $"Matches a {inputs.TimeAvailable} window with {inputs.CompanionType.ToLowerInvariant()} company.",
                RideSlug: "coorg-loop"),
        ];
        return Task.FromResult(recommendations);
    }

    public Task<TripBriefingDto> GenerateTripBriefingAsync(TripBriefingContext context, CancellationToken ct = default)
    {
        var hours = context.RideDurationMinutes / 60.0;
        var narrative =
            $"From {context.Origin} to {context.Destination}: {context.DistanceKm:0.#} km, roughly {hours:0.#} hours in the saddle" +
            (context.Motorcycle is null ? "." : $" on the {context.Motorcycle}.");

        return Task.FromResult(new TripBriefingDto(
            Destination: context.Destination,
            Origin: context.Origin,
            DistanceKm: context.DistanceKm,
            RideDurationMinutes: context.RideDurationMinutes,
            Motorcycle: context.Motorcycle,
            Weather: null,
            FuelEstimate: (decimal)(context.DistanceKm * 4.5),
            BudgetEstimate: (decimal)(context.DistanceKm * 8),
            RoadCondition: "Mixed",
            Difficulty: context.RidingStyle,
            ScenicStops: [],
            FoodStops: [],
            Hotels: [],
            PhotographySpots: [],
            PackingSuggestions: ["Riding jacket", "Rain gear", "Puncture repair kit"],
            SafetyAdvice: ["Check tyre pressure before departure", "Carry a printed/offline copy of your route"],
            AiNarrative: narrative));
    }
}
