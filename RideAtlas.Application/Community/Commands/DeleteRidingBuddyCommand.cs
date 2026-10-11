using MediatR;
using RideAtlas.Application.Common.Exceptions;
using RideAtlas.Application.Common.Interfaces;

namespace RideAtlas.Application.Community.Commands;

public record DeleteRidingBuddyCommand(Guid Id, Guid RiderAccountId) : IRequest;

public class DeleteRidingBuddyCommandHandler(IRidingBuddyRepository repository) : IRequestHandler<DeleteRidingBuddyCommand>
{
    public async Task Handle(DeleteRidingBuddyCommand request, CancellationToken ct)
    {
        var buddy = await repository.GetByIdAsync(request.Id, request.RiderAccountId, ct)
            ?? throw new NotFoundException(nameof(Domain.Entities.RidingBuddy), request.Id);
        await repository.DeleteAsync(buddy, ct);
    }
}
