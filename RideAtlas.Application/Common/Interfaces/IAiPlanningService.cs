using RideAtlas.Application.AiPlanning.Dtos;

namespace RideAtlas.Application.Common.Interfaces;

/// <summary>Wraps the self-hosted Ollama/Qwen model per §7.2. Implementations are swappable —
/// see RideAtlas.Infrastructure for the current placeholder pending real model wiring.</summary>
public interface IAiPlanningService
{
    Task<string> GenerateJournalNarrativeAsync(JournalNarrativeContext context, CancellationToken ct = default);
    Task<JournalDraftDto> GenerateJournalDraftAsync(string riderPrompt, CancellationToken ct = default);
    Task<RideStoryDto> GenerateRideStoryAsync(RideStoryContext context, CancellationToken ct = default);
    Task<IReadOnlyList<TripRecommendationDto>> RecommendTripsAsync(RidePlanningInputsDto inputs, CancellationToken ct = default);
    Task<TripBriefingDto> GenerateTripBriefingAsync(TripBriefingContext context, CancellationToken ct = default);
}
