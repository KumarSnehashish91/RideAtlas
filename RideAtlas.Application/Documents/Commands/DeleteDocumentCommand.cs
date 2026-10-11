using MediatR;
using RideAtlas.Application.Common.Exceptions;
using RideAtlas.Application.Common.Interfaces;

namespace RideAtlas.Application.Documents.Commands;

public record DeleteDocumentCommand(Guid Id, Guid RiderAccountId) : IRequest;

public class DeleteDocumentCommandHandler(IDocumentRepository repository) : IRequestHandler<DeleteDocumentCommand>
{
    public async Task Handle(DeleteDocumentCommand request, CancellationToken ct)
    {
        var document = await repository.GetByIdAsync(request.Id, request.RiderAccountId, ct)
            ?? throw new NotFoundException(nameof(Domain.Entities.Document), request.Id);
        await repository.DeleteAsync(document, ct);
    }
}
