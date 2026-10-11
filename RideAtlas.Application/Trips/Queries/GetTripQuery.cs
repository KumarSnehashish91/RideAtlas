using MediatR;
using RideAtlas.Application.Common.Exceptions;
using RideAtlas.Application.Common.Interfaces;
using RideAtlas.Application.Trips.Dtos;
using RideAtlas.Domain.Entities;

namespace RideAtlas.Application.Trips.Queries;

public record GetTripQuery(Guid Id, Guid RiderAccountId) : IRequest<TripDto>;

public class GetTripQueryHandler(ITripRepository repository, IDateTimeProvider clock)
    : IRequestHandler<GetTripQuery, TripDto>
{
    public async Task<TripDto> Handle(GetTripQuery request, CancellationToken ct)
    {
        var trip = await repository.GetByIdAsync(request.Id, request.RiderAccountId, ct)
            ?? throw new NotFoundException(nameof(Trip), request.Id);
        return trip.ToDto(clock.Today);
    }
}
