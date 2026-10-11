using MediatR;
using RideAtlas.Application.Common.Exceptions;
using RideAtlas.Application.Common.Interfaces;

namespace RideAtlas.Application.ServiceHistory.Commands;

public record DeleteServiceEntryCommand(Guid Id, Guid RiderAccountId) : IRequest;

public class DeleteServiceEntryCommandHandler(IServiceEntryRepository repository) : IRequestHandler<DeleteServiceEntryCommand>
{
    public async Task Handle(DeleteServiceEntryCommand request, CancellationToken ct)
    {
        var entry = await repository.GetByIdAsync(request.Id, request.RiderAccountId, ct)
            ?? throw new NotFoundException(nameof(Domain.Entities.ServiceEntry), request.Id);
        await repository.DeleteAsync(entry, ct);
    }
}
