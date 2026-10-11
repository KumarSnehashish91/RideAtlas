using MediatR;
using RideAtlas.Application.Common.Interfaces;

namespace RideAtlas.Application.ServiceHistory.Queries;

public record ServiceSpendByCategoryDto(string Category, decimal TotalCost, int Count);

public record ServiceAnalyticsDto(
    decimal TotalSpend,
    decimal AverageCostPerService,
    IReadOnlyList<ServiceSpendByCategoryDto> SpendByCategory);

public record ServiceAnalyticsQuery(Guid RiderAccountId, Guid BikeId) : IRequest<ServiceAnalyticsDto>;

public class ServiceAnalyticsQueryHandler(IServiceEntryRepository repository)
    : IRequestHandler<ServiceAnalyticsQuery, ServiceAnalyticsDto>
{
    public async Task<ServiceAnalyticsDto> Handle(ServiceAnalyticsQuery request, CancellationToken ct)
    {
        var entries = await repository.ListAllForBikeAsync(request.RiderAccountId, request.BikeId, ct);

        var totalSpend = entries.Sum(e => e.Cost);
        var average = entries.Count > 0 ? totalSpend / entries.Count : 0m;

        var byCategory = entries
            .GroupBy(e => e.Category)
            .Select(g => new ServiceSpendByCategoryDto(g.Key, g.Sum(e => e.Cost), g.Count()))
            .OrderByDescending(x => x.TotalCost)
            .ToList();

        return new ServiceAnalyticsDto(totalSpend, average, byCategory);
    }
}
