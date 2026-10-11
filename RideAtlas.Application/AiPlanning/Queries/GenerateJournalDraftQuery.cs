using MediatR;
using RideAtlas.Application.AiPlanning.Dtos;
using RideAtlas.Application.Common.Interfaces;

namespace RideAtlas.Application.AiPlanning.Queries;

public record GenerateJournalDraftQuery(string RiderPrompt) : IRequest<JournalDraftDto>;

public class GenerateJournalDraftQueryHandler(IAiPlanningService aiPlanningService)
    : IRequestHandler<GenerateJournalDraftQuery, JournalDraftDto>
{
    public Task<JournalDraftDto> Handle(GenerateJournalDraftQuery request, CancellationToken ct) =>
        aiPlanningService.GenerateJournalDraftAsync(request.RiderPrompt, ct);
}
