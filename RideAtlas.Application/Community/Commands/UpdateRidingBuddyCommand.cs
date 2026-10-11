using FluentValidation;
using MediatR;
using RideAtlas.Application.Common.Exceptions;
using RideAtlas.Application.Common.Interfaces;
using RideAtlas.Application.Community.Dtos;

namespace RideAtlas.Application.Community.Commands;

public record UpdateRidingBuddyCommand(
    Guid Id,
    Guid RiderAccountId,
    string Name,
    string? HomeCity,
    string? AvatarUri,
    string? Notes,
    bool IsFollowing) : IRequest<RidingBuddyDto>;

public class UpdateRidingBuddyCommandValidator : AbstractValidator<UpdateRidingBuddyCommand>
{
    public UpdateRidingBuddyCommandValidator()
    {
        RuleFor(x => x.Name).NotEmpty().MaximumLength(100);
    }
}

public class UpdateRidingBuddyCommandHandler(IRidingBuddyRepository repository)
    : IRequestHandler<UpdateRidingBuddyCommand, RidingBuddyDto>
{
    public async Task<RidingBuddyDto> Handle(UpdateRidingBuddyCommand request, CancellationToken ct)
    {
        var buddy = await repository.GetByIdAsync(request.Id, request.RiderAccountId, ct)
            ?? throw new NotFoundException(nameof(Domain.Entities.RidingBuddy), request.Id);

        buddy.Name = request.Name.Trim();
        buddy.HomeCity = request.HomeCity;
        buddy.AvatarUri = request.AvatarUri;
        buddy.Notes = request.Notes;
        buddy.IsFollowing = request.IsFollowing;

        await repository.UpdateAsync(buddy, ct);
        return buddy.ToDto();
    }
}
