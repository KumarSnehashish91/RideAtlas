using MediatR;
using RideAtlas.Application.Common.Exceptions;
using RideAtlas.Application.Common.Interfaces;
using RideAtlas.Application.Journal.Dtos;

namespace RideAtlas.Application.Journal.Queries;

public record GetJournalEntryQuery(Guid Id, Guid RiderAccountId) : IRequest<JournalEntryDto>;

public class GetJournalEntryQueryHandler(IJournalEntryRepository repository)
    : IRequestHandler<GetJournalEntryQuery, JournalEntryDto>
{
    public async Task<JournalEntryDto> Handle(GetJournalEntryQuery request, CancellationToken ct)
    {
        var entry = await repository.GetByIdAsync(request.Id, request.RiderAccountId, ct)
            ?? throw new NotFoundException(nameof(Domain.Entities.JournalEntry), request.Id);
        return entry.ToDto();
    }
}
