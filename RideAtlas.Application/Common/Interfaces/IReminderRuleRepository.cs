using RideAtlas.Domain.Entities;

namespace RideAtlas.Application.Common.Interfaces;

public interface IReminderRuleRepository
{
    Task<ReminderRule?> GetByIdAsync(Guid id, Guid riderAccountId, CancellationToken ct = default);
    Task<IReadOnlyList<ReminderRule>> ListForBikeAsync(Guid bikeId, Guid riderAccountId, CancellationToken ct = default);
    Task<IReadOnlyList<ReminderRule>> ListAllForRiderAsync(Guid riderAccountId, CancellationToken ct = default);
    Task AddAsync(ReminderRule rule, CancellationToken ct = default);
    Task UpdateAsync(ReminderRule rule, CancellationToken ct = default);
    Task DeleteAsync(ReminderRule rule, CancellationToken ct = default);
}
