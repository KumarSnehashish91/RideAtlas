using FluentValidation;
using MediatR;
using RideAtlas.Application.Common.Exceptions;
using RideAtlas.Application.Common.Interfaces;
using RideAtlas.Application.Reminders.Dtos;

namespace RideAtlas.Application.Reminders.Commands;

public record UpdateReminderRuleCommand(
    Guid Id,
    Guid RiderAccountId,
    string Category,
    string? CustomLabel,
    double? IntervalKm,
    int? IntervalDays,
    double BaselineOdometerKm,
    DateOnly BaselineDate,
    string? Notes) : IRequest<ReminderRuleDto>;

public class UpdateReminderRuleCommandValidator : AbstractValidator<UpdateReminderRuleCommand>
{
    public UpdateReminderRuleCommandValidator()
    {
        RuleFor(x => x.Category).NotEmpty().MaximumLength(50);
        RuleFor(x => x.BaselineOdometerKm).GreaterThanOrEqualTo(0);
        RuleFor(x => x.IntervalKm).GreaterThan(0).When(x => x.IntervalKm.HasValue);
        RuleFor(x => x.IntervalDays).GreaterThan(0).When(x => x.IntervalDays.HasValue);
        RuleFor(x => x)
            .Must(x => x.IntervalKm.HasValue || x.IntervalDays.HasValue)
            .WithMessage("Either IntervalKm or IntervalDays must be provided.");
    }
}

public class UpdateReminderRuleCommandHandler(IReminderRuleRepository repository)
    : IRequestHandler<UpdateReminderRuleCommand, ReminderRuleDto>
{
    public async Task<ReminderRuleDto> Handle(UpdateReminderRuleCommand request, CancellationToken ct)
    {
        var rule = await repository.GetByIdAsync(request.Id, request.RiderAccountId, ct)
            ?? throw new NotFoundException(nameof(Domain.Entities.ReminderRule), request.Id);

        rule.Category = request.Category.Trim();
        rule.CustomLabel = request.CustomLabel;
        rule.IntervalKm = request.IntervalKm;
        rule.IntervalDays = request.IntervalDays;
        rule.BaselineOdometerKm = request.BaselineOdometerKm;
        rule.BaselineDate = request.BaselineDate;
        rule.Notes = request.Notes;

        await repository.UpdateAsync(rule, ct);
        return rule.ToDto();
    }
}
