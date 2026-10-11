using MediatR;
using RideAtlas.Application.Common.Interfaces;
using RideAtlas.Application.Documents.Dtos;

namespace RideAtlas.Application.Documents.Queries;

public record ListExpiringSoonDocumentsQuery(Guid RiderAccountId, int WithinDays) : IRequest<IReadOnlyList<DocumentDto>>;

public class ListExpiringSoonDocumentsQueryHandler(IDocumentRepository repository, IDateTimeProvider clock)
    : IRequestHandler<ListExpiringSoonDocumentsQuery, IReadOnlyList<DocumentDto>>
{
    public async Task<IReadOnlyList<DocumentDto>> Handle(ListExpiringSoonDocumentsQuery request, CancellationToken ct)
    {
        var withinDays = request.WithinDays <= 0 ? 30 : request.WithinDays;
        var documents = await repository.ListExpiringSoonAsync(request.RiderAccountId, clock.Today, withinDays, ct);
        return documents.Select(d => d.ToDto()).ToList();
    }
}
