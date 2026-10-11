using MediatR;
using RideAtlas.Application.Common.Exceptions;
using RideAtlas.Application.Common.Interfaces;
using RideAtlas.Application.Events.Dtos;

namespace RideAtlas.Application.Events.Queries;

public record GetRideEventQuery(Guid Id, Guid RiderAccountId) : IRequest<RideEventDto>;

public class GetRideEventQueryHandler(IRideEventRepository repository)
    : IRequestHandler<GetRideEventQuery, RideEventDto>
{
    public async Task<RideEventDto> Handle(GetRideEventQuery request, CancellationToken ct)
    {
        var rideEvent = await repository.GetByIdAsync(request.Id, request.RiderAccountId, ct)
            ?? throw new NotFoundException(nameof(Domain.Entities.RideEvent), request.Id);
        return rideEvent.ToDto();
    }
}
