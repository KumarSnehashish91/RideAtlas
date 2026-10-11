using RideAtlas.Domain.Entities;

namespace RideAtlas.Application.Common.Interfaces;

public interface IEventInviteeRepository
{
    Task<EventInvitee?> GetByIdAsync(Guid id, CancellationToken ct = default);
    Task<IReadOnlyList<EventInvitee>> ListForEventAsync(Guid eventId, CancellationToken ct = default);
    Task AddAsync(EventInvitee invitee, CancellationToken ct = default);
    Task UpdateAsync(EventInvitee invitee, CancellationToken ct = default);
    Task DeleteAsync(EventInvitee invitee, CancellationToken ct = default);
}
