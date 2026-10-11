using MediatR;
using RideAtlas.Application.Common.Exceptions;
using RideAtlas.Application.Common.Interfaces;

namespace RideAtlas.Application.Journal.Commands;

public record DeleteJournalEntryCommand(Guid Id, Guid RiderAccountId) : IRequest;

public class DeleteJournalEntryCommandHandler(IJournalEntryRepository repository) : IRequestHandler<DeleteJournalEntryCommand>
{
    public async Task Handle(DeleteJournalEntryCommand request, CancellationToken ct)
    {
        var entry = await repository.GetByIdAsync(request.Id, request.RiderAccountId, ct)
            ?? throw new NotFoundException(nameof(Domain.Entities.JournalEntry), request.Id);
        await repository.DeleteAsync(entry, ct);
    }
}
