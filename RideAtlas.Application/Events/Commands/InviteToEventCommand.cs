using FluentValidation;
using MediatR;
using RideAtlas.Application.Common.Exceptions;
using RideAtlas.Application.Common.Interfaces;
using RideAtlas.Application.Events.Dtos;
using RideAtlas.Domain.Entities;

namespace RideAtlas.Application.Events.Commands;

public record InviteToEventCommand(Guid EventId, Guid RiderAccountId, Guid BuddyId) : IRequest<EventInviteeDto>;

public class InviteToEventCommandValidator : AbstractValidator<InviteToEventCommand>
{
    public InviteToEventCommandValidator()
    {
        RuleFor(x => x.BuddyId).NotEmpty();
    }
}

public class InviteToEventCommandHandler(
    IRideEventRepository eventRepository,
    IRidingBuddyRepository buddyRepository,
    IEventInviteeRepository inviteeRepository) : IRequestHandler<InviteToEventCommand, EventInviteeDto>
{
    public async Task<EventInviteeDto> Handle(InviteToEventCommand request, CancellationToken ct)
    {
        _ = await eventRepository.GetByIdAsync(request.EventId, request.RiderAccountId, ct)
            ?? throw new NotFoundException(nameof(RideEvent), request.EventId);
        _ = await buddyRepository.GetByIdAsync(request.BuddyId, request.RiderAccountId, ct)
            ?? throw new NotFoundException(nameof(RidingBuddy), request.BuddyId);

        var invitee = new EventInvitee { EventId = request.EventId, BuddyId = request.BuddyId };
        await inviteeRepository.AddAsync(invitee, ct);
        return invitee.ToDto();
    }
}
