using MediatR;
using RideAtlas.Application.Common.Exceptions;
using RideAtlas.Application.Common.Interfaces;

namespace RideAtlas.Application.Packing.Commands;

public record DeletePackingItemCommand(Guid Id, Guid RiderAccountId) : IRequest;

public class DeletePackingItemCommandHandler(IPackingItemRepository repository) : IRequestHandler<DeletePackingItemCommand>
{
    public async Task Handle(DeletePackingItemCommand request, CancellationToken ct)
    {
        var item = await repository.GetByIdAsync(request.Id, request.RiderAccountId, ct)
            ?? throw new NotFoundException(nameof(Domain.Entities.PackingItem), request.Id);
        await repository.DeleteAsync(item, ct);
    }
}
