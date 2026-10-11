using MediatR;
using RideAtlas.Application.Common.Exceptions;
using RideAtlas.Application.Common.Interfaces;
using RideAtlas.Application.Rides.Dtos;
using RideAtlas.Domain.Entities;

namespace RideAtlas.Application.Rides.Queries;

public record GetRideQuery(Guid Id, Guid RiderAccountId) : IRequest<RideDto>;

public class GetRideQueryHandler(IRideRepository repository) : IRequestHandler<GetRideQuery, RideDto>
{
    public async Task<RideDto> Handle(GetRideQuery request, CancellationToken ct)
    {
        var ride = await repository.GetByIdAsync(request.Id, request.RiderAccountId, ct)
            ?? throw new NotFoundException(nameof(Ride), request.Id);
        return ride.ToDto();
    }
}
