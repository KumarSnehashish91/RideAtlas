using MediatR;
using RideAtlas.Application.Common.Exceptions;
using RideAtlas.Application.Common.Interfaces;

namespace RideAtlas.Application.Reminders.Commands;

public record DeleteReminderRuleCommand(Guid Id, Guid RiderAccountId) : IRequest;

public class DeleteReminderRuleCommandHandler(IReminderRuleRepository repository) : IRequestHandler<DeleteReminderRuleCommand>
{
    public async Task Handle(DeleteReminderRuleCommand request, CancellationToken ct)
    {
        var rule = await repository.GetByIdAsync(request.Id, request.RiderAccountId, ct)
            ?? throw new NotFoundException(nameof(Domain.Entities.ReminderRule), request.Id);
        await repository.DeleteAsync(rule, ct);
    }
}
