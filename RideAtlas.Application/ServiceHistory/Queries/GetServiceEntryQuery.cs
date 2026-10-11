using MediatR;
using RideAtlas.Application.Common.Exceptions;
using RideAtlas.Application.Common.Interfaces;
using RideAtlas.Application.ServiceHistory.Dtos;

namespace RideAtlas.Application.ServiceHistory.Queries;

public record GetServiceEntryQuery(Guid Id, Guid RiderAccountId) : IRequest<ServiceEntryDto>;

public class GetServiceEntryQueryHandler(IServiceEntryRepository repository)
    : IRequestHandler<GetServiceEntryQuery, ServiceEntryDto>
{
    public async Task<ServiceEntryDto> Handle(GetServiceEntryQuery request, CancellationToken ct)
    {
        var entry = await repository.GetByIdAsync(request.Id, request.RiderAccountId, ct)
            ?? throw new NotFoundException(nameof(Domain.Entities.ServiceEntry), request.Id);
        return entry.ToDto();
    }
}
