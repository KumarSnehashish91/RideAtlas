using MediatR;
using RideAtlas.Application.Common.Exceptions;
using RideAtlas.Application.Common.Interfaces;
using RideAtlas.Application.Documents.Dtos;

namespace RideAtlas.Application.Documents.Queries;

public record GetDocumentQuery(Guid Id, Guid RiderAccountId) : IRequest<DocumentDto>;

public class GetDocumentQueryHandler(IDocumentRepository repository) : IRequestHandler<GetDocumentQuery, DocumentDto>
{
    public async Task<DocumentDto> Handle(GetDocumentQuery request, CancellationToken ct)
    {
        var document = await repository.GetByIdAsync(request.Id, request.RiderAccountId, ct)
            ?? throw new NotFoundException(nameof(Domain.Entities.Document), request.Id);
        return document.ToDto();
    }
}
