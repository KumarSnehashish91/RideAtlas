using MediatR;
using RideAtlas.Application.Common.Exceptions;
using RideAtlas.Application.Common.Interfaces;
using RideAtlas.Application.Reminders.Dtos;
using RideAtlas.Domain.Entities;

namespace RideAtlas.Application.Reminders.Commands;

public record MarkReminderRuleDoneCommand(Guid Id, Guid RiderAccountId) : IRequest<ReminderRuleDto>;

public class MarkReminderRuleDoneCommandHandler(
    IReminderRuleRepository repository,
    IBikeRepository bikeRepository,
    IDateTimeProvider clock) : IRequestHandler<MarkReminderRuleDoneCommand, ReminderRuleDto>
{
    public async Task<ReminderRuleDto> Handle(MarkReminderRuleDoneCommand request, CancellationToken ct)
    {
        var rule = await repository.GetByIdAsync(request.Id, request.RiderAccountId, ct)
            ?? throw new NotFoundException(nameof(Domain.Entities.ReminderRule), request.Id);

        var bike = await bikeRepository.GetByIdAsync(rule.BikeId, request.RiderAccountId, ct)
            ?? throw new NotFoundException(nameof(Motorcycle), rule.BikeId);

        rule.BaselineOdometerKm = bike.CurrentOdometerKm;
        rule.BaselineDate = clock.Today;

        await repository.UpdateAsync(rule, ct);
        return rule.ToDto();
    }
}
