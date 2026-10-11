using FluentValidation;
using MediatR;
using RideAtlas.Application.Common.Interfaces;
using RideAtlas.Application.Community.Dtos;
using RideAtlas.Domain.Entities;

namespace RideAtlas.Application.Community.Commands;

public record UpdateMyProfileCommand(
    Guid RiderAccountId,
    string DisplayName,
    string? Bio,
    string? HomeCity,
    string? AvatarUri,
    int? RidingSinceYear,
    bool IsPublic,
    bool ShowLocation,
    bool ShowStats,
    bool ShowBikes) : IRequest<RiderProfileDto>;

public class UpdateMyProfileCommandValidator : AbstractValidator<UpdateMyProfileCommand>
{
    public UpdateMyProfileCommandValidator()
    {
        RuleFor(x => x.DisplayName).NotEmpty().MaximumLength(100);
        RuleFor(x => x.Bio).MaximumLength(1000);
    }
}

public class UpdateMyProfileCommandHandler(IRiderProfileRepository repository)
    : IRequestHandler<UpdateMyProfileCommand, RiderProfileDto>
{
    public async Task<RiderProfileDto> Handle(UpdateMyProfileCommand request, CancellationToken ct)
    {
        var profile = await repository.GetByRiderAccountIdAsync(request.RiderAccountId, ct);
        if (profile is null)
        {
            profile = new RiderProfile { RiderAccountId = request.RiderAccountId };
            Apply(profile, request);
            await repository.AddAsync(profile, ct);
        }
        else
        {
            Apply(profile, request);
            await repository.UpdateAsync(profile, ct);
        }
        return profile.ToDto();
    }

    private static void Apply(RiderProfile profile, UpdateMyProfileCommand request)
    {
        profile.DisplayName = request.DisplayName.Trim();
        profile.Bio = request.Bio;
        profile.HomeCity = request.HomeCity;
        profile.AvatarUri = request.AvatarUri;
        profile.RidingSinceYear = request.RidingSinceYear;
        profile.IsPublic = request.IsPublic;
        profile.ShowLocation = request.ShowLocation;
        profile.ShowStats = request.ShowStats;
        profile.ShowBikes = request.ShowBikes;
    }
}
