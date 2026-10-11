using MediatR;
using RideAtlas.Application.Common.Exceptions;
using RideAtlas.Application.Common.Interfaces;
using RideAtlas.Domain.Entities;

namespace RideAtlas.Application.Trips.Commands;

public record DeleteTripCommand(Guid Id, Guid RiderAccountId) : IRequest;

public class DeleteTripCommandHandler(ITripRepository repository) : IRequestHandler<DeleteTripCommand>
{
    public async Task Handle(DeleteTripCommand request, CancellationToken ct)
    {
        var trip = await repository.GetByIdAsync(request.Id, request.RiderAccountId, ct)
            ?? throw new NotFoundException(nameof(Trip), request.Id);
        await repository.DeleteAsync(trip, ct);
    }
}
