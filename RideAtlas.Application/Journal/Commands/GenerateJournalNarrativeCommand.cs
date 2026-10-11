using MediatR;
using RideAtlas.Application.AiPlanning.Dtos;
using RideAtlas.Application.Common.Exceptions;
using RideAtlas.Application.Common.Interfaces;
using RideAtlas.Application.Journal.Dtos;

namespace RideAtlas.Application.Journal.Commands;

public record GenerateJournalNarrativeCommand(Guid Id, Guid RiderAccountId) : IRequest<JournalEntryDto>;

public class GenerateJournalNarrativeCommandHandler(
    IJournalEntryRepository repository,
    IAiPlanningService aiPlanningService) : IRequestHandler<GenerateJournalNarrativeCommand, JournalEntryDto>
{
    public async Task<JournalEntryDto> Handle(GenerateJournalNarrativeCommand request, CancellationToken ct)
    {
        var entry = await repository.GetByIdAsync(request.Id, request.RiderAccountId, ct)
            ?? throw new NotFoundException(nameof(Domain.Entities.JournalEntry), request.Id);

        var context = new JournalNarrativeContext(entry.Title, entry.Date, entry.Mood, entry.DistanceKm, entry.RiderInputRaw);
        entry.AiGeneratedStory = await aiPlanningService.GenerateJournalNarrativeAsync(context, ct);

        await repository.UpdateAsync(entry, ct);
        return entry.ToDto();
    }
}
