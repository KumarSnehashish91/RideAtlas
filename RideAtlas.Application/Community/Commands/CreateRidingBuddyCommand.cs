using FluentValidation;
using MediatR;
using RideAtlas.Application.Common.Interfaces;
using RideAtlas.Application.Community.Dtos;
using RideAtlas.Domain.Entities;

namespace RideAtlas.Application.Community.Commands;

public record CreateRidingBuddyCommand(
    Guid RiderAccountId,
    string Name,
    string? HomeCity,
    string? AvatarUri,
    string? Notes,
    Guid? RemoteRiderId) : IRequest<RidingBuddyDto>;

public class CreateRidingBuddyCommandValidator : AbstractValidator<CreateRidingBuddyCommand>
{
    public CreateRidingBuddyCommandValidator()
    {
        RuleFor(x => x.Name).NotEmpty().MaximumLength(100);
    }
}

public class CreateRidingBuddyCommandHandler(IRidingBuddyRepository repository)
    : IRequestHandler<CreateRidingBuddyCommand, RidingBuddyDto>
{
    public async Task<RidingBuddyDto> Handle(CreateRidingBuddyCommand request, CancellationToken ct)
    {
        var buddy = new RidingBuddy
        {
            RiderAccountId = request.RiderAccountId,
            Name = request.Name.Trim(),
            HomeCity = request.HomeCity,
            AvatarUri = request.AvatarUri,
            Notes = request.Notes,
            RemoteRiderId = request.RemoteRiderId
        };
        await repository.AddAsync(buddy, ct);
        return buddy.ToDto();
    }
}
