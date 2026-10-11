using MediatR;
using RideAtlas.Application.Common.Interfaces;
using RideAtlas.Application.Reminders.Dtos;

namespace RideAtlas.Application.Reminders.Queries;

public record ListReminderRulesQuery(Guid RiderAccountId, Guid BikeId) : IRequest<IReadOnlyList<ReminderRuleDto>>;

public class ListReminderRulesQueryHandler(IReminderRuleRepository repository)
    : IRequestHandler<ListReminderRulesQuery, IReadOnlyList<ReminderRuleDto>>
{
    public async Task<IReadOnlyList<ReminderRuleDto>> Handle(ListReminderRulesQuery request, CancellationToken ct)
    {
        var rules = await repository.ListForBikeAsync(request.BikeId, request.RiderAccountId, ct);
        return rules.Select(r => r.ToDto()).ToList();
    }
}
