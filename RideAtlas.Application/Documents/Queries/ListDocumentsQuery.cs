using MediatR;
using RideAtlas.Application.Common.Interfaces;
using RideAtlas.Application.Documents.Dtos;

namespace RideAtlas.Application.Documents.Queries;

public record ListDocumentsQuery(Guid RiderAccountId, Guid? BikeId) : IRequest<IReadOnlyList<DocumentDto>>;

public class ListDocumentsQueryHandler(IDocumentRepository repository)
    : IRequestHandler<ListDocumentsQuery, IReadOnlyList<DocumentDto>>
{
    public async Task<IReadOnlyList<DocumentDto>> Handle(ListDocumentsQuery request, CancellationToken ct)
    {
        var documents = await repository.ListAsync(request.RiderAccountId, request.BikeId, ct);
        return documents.Select(d => d.ToDto()).ToList();
    }
}
