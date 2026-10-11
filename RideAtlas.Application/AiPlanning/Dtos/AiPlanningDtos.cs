namespace RideAtlas.Application.AiPlanning.Dtos;

public record RidePlanningInputsDto(
    string CompanionType,
    IReadOnlyList<string> Interests,
    string TimeAvailable,
    string RidingStyle);

public record TripRecommendationDto(
    string Destination,
    string Region,
    string Summary,
    string Difficulty,
    string RoadType,
    string BestSeason,
    double ScenicRating,
    decimal FuelEstimate,
    decimal BudgetEstimate,
    string WeatherOverview,
    string RecommendationReason,
    string? RideSlug);

public record TripBriefingDto(
    string Destination,
    string Origin,
    double DistanceKm,
    int RideDurationMinutes,
    string? Motorcycle,
    string? Weather,
    decimal FuelEstimate,
    decimal BudgetEstimate,
    string RoadCondition,
    string Difficulty,
    IReadOnlyList<string> ScenicStops,
    IReadOnlyList<string> FoodStops,
    IReadOnlyList<string> Hotels,
    IReadOnlyList<string> PhotographySpots,
    IReadOnlyList<string> PackingSuggestions,
    IReadOnlyList<string> SafetyAdvice,
    string AiNarrative);

public record RideStoryDto(
    string JourneySummary,
    IReadOnlyList<string> Highlights,
    IReadOnlyList<string> InterestingStatistics,
    IReadOnlyList<string> MemorableMoments,
    IReadOnlyList<string> RidingInsights,
    IReadOnlyList<string> SafetyObservations,
    IReadOnlyList<string> FutureSuggestions,
    string NarrativeText);

public record JournalDraftDto(string Title, string Mood, string NarrativeText);

public record JournalNarrativeContext(
    string Title,
    DateOnly Date,
    string? Mood,
    double? DistanceKm,
    string? RiderInputRaw);

public record RideStoryContext(
    double DistanceKm,
    double DurationMinutes,
    double? AvgSpeedKmh,
    double? MaxSpeedKmh,
    string? RouteDescription,
    string? WeatherCondition,
    string? Notes);

public record TripBriefingContext(
    string Destination,
    string Origin,
    double DistanceKm,
    int RideDurationMinutes,
    string? Motorcycle,
    string RidingStyle);
