using Microsoft.EntityFrameworkCore;
using RideAtlas.Application.Common.Interfaces;
using RideAtlas.Domain.Entities;

namespace RideAtlas.Infrastructure.Persistence.Repositories;

public class EventInviteeRepository(RideAtlasDbContext db) : IEventInviteeRepository
{
    public Task<EventInvitee?> GetByIdAsync(Guid id, CancellationToken ct = default) =>
        db.EventInvitees.FirstOrDefaultAsync(x => x.Id == id, ct);

    public async Task<IReadOnlyList<EventInvitee>> ListForEventAsync(Guid eventId, CancellationToken ct = default) =>
        await db.EventInvitees.Where(x => x.EventId == eventId).ToListAsync(ct);

    public async Task AddAsync(EventInvitee invitee, CancellationToken ct = default)
    {
        db.EventInvitees.Add(invitee);
        await db.SaveChangesAsync(ct);
    }

    public async Task UpdateAsync(EventInvitee invitee, CancellationToken ct = default) =>
        await db.SaveChangesAsync(ct);

    public async Task DeleteAsync(EventInvitee invitee, CancellationToken ct = default)
    {
        db.EventInvitees.Remove(invitee);
        await db.SaveChangesAsync(ct);
    }
}
