using FluentValidation;
using MediatR;
using RideAtlas.Application.Common.Exceptions;
using RideAtlas.Application.Common.Interfaces;
using RideAtlas.Application.Events.Dtos;

namespace RideAtlas.Application.Events.Commands;

public record RespondToInviteCommand(Guid InviteeId, string RsvpStatus) : IRequest<EventInviteeDto>;

public class RespondToInviteCommandValidator : AbstractValidator<RespondToInviteCommand>
{
    private static readonly string[] AllowedStatuses = ["Invited", "Accepted", "Declined", "Maybe"];

    public RespondToInviteCommandValidator()
    {
        RuleFor(x => x.RsvpStatus).Must(s => AllowedStatuses.Contains(s))
            .WithMessage($"RsvpStatus must be one of: {string.Join(", ", AllowedStatuses)}");
    }
}

public class RespondToInviteCommandHandler(IEventInviteeRepository repository)
    : IRequestHandler<RespondToInviteCommand, EventInviteeDto>
{
    public async Task<EventInviteeDto> Handle(RespondToInviteCommand request, CancellationToken ct)
    {
        var invitee = await repository.GetByIdAsync(request.InviteeId, ct)
            ?? throw new NotFoundException(nameof(Domain.Entities.EventInvitee), request.InviteeId);

        invitee.RsvpStatus = request.RsvpStatus;
        await repository.UpdateAsync(invitee, ct);
        return invitee.ToDto();
    }
}
