using FluentValidation;
using MediatR;
using RideAtlas.Application.Common.Exceptions;
using RideAtlas.Application.Common.Interfaces;
using RideAtlas.Application.Reminders.Dtos;
using RideAtlas.Domain.Entities;

namespace RideAtlas.Application.Reminders.Commands;

public record CreateReminderRuleCommand(
    Guid RiderAccountId,
    Guid BikeId,
    string Category,
    string? CustomLabel,
    double? IntervalKm,
    int? IntervalDays,
    double BaselineOdometerKm,
    DateOnly BaselineDate,
    string? Notes) : IRequest<ReminderRuleDto>;

public class CreateReminderRuleCommandValidator : AbstractValidator<CreateReminderRuleCommand>
{
    public CreateReminderRuleCommandValidator()
    {
        RuleFor(x => x.BikeId).NotEmpty();
        RuleFor(x => x.Category).NotEmpty().MaximumLength(50);
        RuleFor(x => x.BaselineOdometerKm).GreaterThanOrEqualTo(0);
        RuleFor(x => x.IntervalKm).GreaterThan(0).When(x => x.IntervalKm.HasValue);
        RuleFor(x => x.IntervalDays).GreaterThan(0).When(x => x.IntervalDays.HasValue);
        RuleFor(x => x)
            .Must(x => x.IntervalKm.HasValue || x.IntervalDays.HasValue)
            .WithMessage("Either IntervalKm or IntervalDays must be provided.");
    }
}

public class CreateReminderRuleCommandHandler(
    IReminderRuleRepository repository,
    IBikeRepository bikeRepository) : IRequestHandler<CreateReminderRuleCommand, ReminderRuleDto>
{
    public async Task<ReminderRuleDto> Handle(CreateReminderRuleCommand request, CancellationToken ct)
    {
        _ = await bikeRepository.GetByIdAsync(request.BikeId, request.RiderAccountId, ct)
            ?? throw new NotFoundException(nameof(Motorcycle), request.BikeId);

        var rule = new ReminderRule
        {
            RiderAccountId = request.RiderAccountId,
            BikeId = request.BikeId,
            Category = request.Category.Trim(),
            CustomLabel = request.CustomLabel,
            IntervalKm = request.IntervalKm,
            IntervalDays = request.IntervalDays,
            BaselineOdometerKm = request.BaselineOdometerKm,
            BaselineDate = request.BaselineDate,
            Notes = request.Notes
        };
        await repository.AddAsync(rule, ct);
        return rule.ToDto();
    }
}
