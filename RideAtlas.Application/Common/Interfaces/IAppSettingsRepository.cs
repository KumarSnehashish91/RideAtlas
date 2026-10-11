using RideAtlas.Domain.Entities;

namespace RideAtlas.Application.Common.Interfaces;

public interface IAppSettingsRepository
{
    Task<AppSettings?> GetByRiderAccountIdAsync(Guid riderAccountId, CancellationToken ct = default);
    Task AddAsync(AppSettings settings, CancellationToken ct = default);
    Task UpdateAsync(AppSettings settings, CancellationToken ct = default);
}
